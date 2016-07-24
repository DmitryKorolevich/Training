using System;
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

            var encryptionHost = Host.Services.GetRequiredService<IObjectEncryptionHost>();

            Console.WriteLine("Starting customer CCs Move");
            RecryptCustomers(encryptionHost);
            Console.WriteLine("Starting orders CCs Move");
            RecryptOrders(encryptionHost);

            Host.Dispose();
        }

        private static void RecryptOrders(IObjectEncryptionHost encryptionHost)
        {
            bool any = true;
            var seed = 0;
            var size = 5000;
            Regex numberCheck = new Regex("^[0-9]+$", RegexOptions.Compiled);
            while (any)
            {
                using (var context = Host.Services.GetRequiredService<ExportInfoContext>())
                {
                    Console.WriteLine($"Moving page: {seed}--{seed + size}");
                    any = false;
                    Parallel.ForEach(context.Set<OrderPaymentMethodExport>().Skip(seed).Take(size), paymentMethod =>
                    {
                        Interlocked.Increment(ref seed);
                        any = true;
                        if (numberCheck.IsMatch(Encoding.Unicode.GetString(paymentMethod.CreditCardNumber)))
                        {
                            paymentMethod.CreditCardNumber = encryptionHost.LocalEncrypt(paymentMethod.CreditCardNumber);
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
                    });

                    context.SaveChanges();
                }
            }
        }

        private static void RecryptCustomers(IObjectEncryptionHost encryptionHost)
        {
            bool any = true;
            var seed = 0;
            var size = 5000;
            Regex numberCheck = new Regex("^[0-9]+$", RegexOptions.Compiled);
            while (any)
            {
                using (var context = Host.Services.GetRequiredService<ExportInfoContext>())
                {
                    Console.WriteLine($"Moving page: {seed}--{seed + size}");
                    any = false;
                    Parallel.ForEach(context.Set<CustomerPaymentMethodExport>().Skip(seed).Take(size), paymentMethod =>
                    {
                        Interlocked.Increment(ref seed);
                        any = true;
                        if (numberCheck.IsMatch(Encoding.Unicode.GetString(paymentMethod.CreditCardNumber)))
                        {
                            paymentMethod.CreditCardNumber = encryptionHost.LocalEncrypt(paymentMethod.CreditCardNumber);
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
                    });

                    context.SaveChanges();
                }
            }
        }
    }
}