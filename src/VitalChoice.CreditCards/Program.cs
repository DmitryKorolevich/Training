using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.CreditCards.Entities;
using Microsoft.EntityFrameworkCore;
using VitalChoice.CreditCards.Services;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;

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

            Console.WriteLine("Press any key to start exporting...");
            Console.ReadKey();

            var exportService = Host.Services.GetRequiredService<IOrderExportService>();
            var orders = File.ReadAllText("fck_list.txt")
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);
            var sync = new object();
            exportService.ExportOrders(orders.Select(o => new OrderExportItem
            {
                OrderType = ExportSide.All,
                IsRefund = false,
                Id = o
            }).ToList(), done =>
            {
                if (done.Success)
                    return;

                lock (sync)
                {
                    Console.WriteLine($"order: {done.Id} filed:\n{done.Error}\n\n");
                }
            }).GetAwaiter().GetResult();
            Console.WriteLine("Done!");
            Console.ReadKey();
            Host.Dispose();
        }

        private static char GetNext(string number)
        {
            int sum = 0;
            for (int i = 1; i <= number.Length; i++)
            {
                var index = number.Length - i;
                if (i%2 != 0)
                {
                    var mul = (number[index] - 48)*2;
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