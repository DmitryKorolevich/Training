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

                //Console.WriteLine("Starting customer CCs Move");
                //RecryptCustomers(encryptionHost);
                //Console.WriteLine("Starting orders CCs Move");
                //RecryptOrders(encryptionHost);

                //Console.WriteLine("Done!");
                using (var context = Host.Services.GetRequiredService<ExportInfoContext>())
                {
                    var uow = new UnitOfWork(context, false);
                    var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                    var orderService = Host.Services.GetRequiredService<IExtendedDynamicReadServiceAsync<OrderDynamic, Order>>();
                    PagedList<OrderDynamic> list;
                    int seed = 0;
                    int page = 3000;
                    do
                    {
                        list =
                            orderService.SelectPage(seed, page,
                                query:
                                c =>
                                    c.StatusCode != 3 &&
                                    (c.IdObjectType == (int) OrderType.AutoShip || c.IdObjectType == (int) OrderType.AutoShipOrder ||
                                     c.OrderStatus == OrderStatus.ShipDelayed || c.OrderStatus == OrderStatus.Processed ||
                                     c.OrderStatus == OrderStatus.OnHold),
                                includesOverride: q => q.Include(c => c.PaymentMethod).ThenInclude(p => p.OptionValues),
                                orderBy: q => q.OrderByDescending(c => c.DateCreated));
                        var failedList = new List<string>();
                        var payments = list.Items.ToDictionary(p => p.Id);
                        var orderIds = payments.Keys.ToList();
                        var ccs = rep.Query(c => orderIds.Contains(c.IdOrder)).Select(true);
                        foreach (var cc in ccs)
                        {
                            var clearCc = encryptionHost.LocalDecrypt<string>(cc.CreditCardNumber);
                            OrderDynamic order;
                            if (clearCc.Length > 4 && payments.TryGetValue(cc.IdOrder, out order))
                            {
                                var card = (string) order.PaymentMethod.SafeData.CardNumber;
                                if (!string.IsNullOrWhiteSpace(card) && card.Length > 4)
                                {
                                    var savedCard = clearCc.Substring(clearCc.Length - 3, 3);
                                    var profileCard = card.Substring(card.Length - 4, 3);
                                    if (savedCard == profileCard &&
                                        clearCc.Substring(clearCc.Length - 4, 4) != card.Substring(card.Length - 4, 4))
                                    {
                                        cc.CreditCardNumber =
                                            encryptionHost.LocalEncrypt(clearCc + card.Substring(card.Length - 1, 1));
                                        failedList.Add($"{cc.IdOrder}, {clearCc + card.Substring(card.Length - 1, 1)}");
                                    }
                                }
                            }
                        }
                        rep.SaveChanges();
                        File.AppendAllLines("C:\\Temp\\failed_orders.txt", failedList);
                        seed++;
                    } while (list.Items.Count > 0);
                }
                Console.ReadKey();
            }
            Host.Dispose();
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