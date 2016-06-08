using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace VitalChoice.CustomerFiles
{
    public class Program
    {
        public static IWebHost Host;

        public static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now:O}] Configuring IoC");
            Host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            Host.Run();
        }
    }
}
