using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.CreditCards.Entities;
using VitalChoice.Infrastructure.ServiceBus.Base;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.CreditCards
{
    public class Program
    {
        public static IWebHost Host;

        public static void Main(string[] args)
        {
            Host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            Host.Start();

            using (var encryptionHost = Host.Services.GetRequiredService<IObjectEncryptionHost>())
            {

                using (var context = Host.Services.GetRequiredService<ExportInfoContext>())
                {
                    var uow = new UnitOfWork(context, false);
                    var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();

                    var idList = args.Select(int.Parse).Distinct().ToList();
                    var orderCards = rep.Query(o => idList.Contains(o.IdOrder)).Select(false);
                    File.WriteAllLines("cards.txt",
                        // ReSharper disable once AccessToDisposedClosure
                        orderCards.Select(card => $"{card.IdOrder}: {encryptionHost.LocalDecrypt<string>(card.CreditCardNumber)}"));
                }
            }
            Host.Dispose();
        }

        private static char GetNext(string number)
        {
            int sum = 0;
            for (int i = 1; i <= number.Length; i++)
            {
                var index = number.Length - i;
                if (i % 2 != 0)
                {
                    var mul = (number[index] - 48) * 2;
                    sum += mul > 9 ? mul - 9 : mul;
                }
                else
                {
                    sum += (number[index] - 48);
                }
            }
            return (char) ((10 - sum%10)%10 + 48);
        }

        private static bool ValidateCardCheckSum(string number)
        {
            int sum = 0;
            for (int i = 1; i <= number.Length; i++)
            {
                var index = number.Length - i;
                if (i%2 == 0)
                {
                    var mul = (number[index] - 48)*2;
                    sum += mul > 9 ? mul - 9 : mul;
                }
                else
                {
                    sum += number[index] - 48;
                }
            }
            return sum%10 == 0;
        }

        private static void RecryptCustomers(IObjectEncryptionHost encryptionHost)
        {
            var seed = 0;
            var size = 20000;
            Regex numberCheck = new Regex("^[0-9]+$", RegexOptions.Compiled);

            List<Task> tasks = new List<Task>();
            bool any = true;
            while (any)
            {
                Console.WriteLine($"Moving page: {seed}--{seed + size}");

                Task task;
                seed = ProcessNextCustomersBatch(encryptionHost, seed, size, numberCheck, out any, out task);
                tasks.Add(task);
            }
            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static void RecryptOrders(IObjectEncryptionHost encryptionHost)
        {
            var seed = 0;
            var size = 20000;
            Regex numberCheck = new Regex("^[0-9]+$", RegexOptions.Compiled);

            List<Task> tasks = new List<Task>();
            bool any = true;
            while (any)
            {
                Console.WriteLine($"Moving page: {seed}--{seed + size}");

                Task task;
                seed = ProcessNextOrdersBatch(encryptionHost, seed, size, numberCheck, out any, out task);
                tasks.Add(task);
            }
            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static int ProcessNextOrdersBatch(IObjectEncryptionHost encryptionHost, int seed, int size, Regex numberCheck,
            out bool hasNext, out Task awaiter)
        {
            var context = Host.Services.GetRequiredService<ExportInfoContext>();
            var items = context.Set<OrderPaymentMethodExport>().Skip(seed).Take(size).ToList();
            seed += items.Count;
            hasNext = items.Count == size;
            awaiter = Task.Run(() => ProcessPayments(encryptionHost, numberCheck, items, context));
            return seed;
        }

        private static int ProcessNextCustomersBatch(IObjectEncryptionHost encryptionHost, int seed, int size, Regex numberCheck,
            out bool hasNext, out Task awaiter)
        {
            var context = Host.Services.GetRequiredService<ExportInfoContext>();
            var items = context.Set<CustomerPaymentMethodExport>().Skip(seed).Take(size).ToList();
            seed += items.Count;
            hasNext = items.Count == size;
            awaiter = Task.Run(() => ProcessPayments(encryptionHost, numberCheck, items, context));
            return seed;
        }

        private static void ProcessPayments(IObjectEncryptionHost encryptionHost, Regex numberCheck, IEnumerable<PaymentMethodExport> items,
            ExportInfoContext context)
        {
            try
            {
                foreach (var paymentMethod in items)
                {
                    var ccNumber = Encoding.Unicode.GetString(paymentMethod.CreditCardNumber);
                    if (numberCheck.IsMatch(ccNumber))
                    {
                        paymentMethod.CreditCardNumber = encryptionHost.LocalEncrypt(ccNumber);
                    }
                    else
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        lock (context)
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            context.SetState(paymentMethod, EntityState.Deleted);
                        }
                    }
                }

                context.SaveChanges();
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}