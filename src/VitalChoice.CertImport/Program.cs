using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VitalChoice.CertImport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var certStore = new X509Store(StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            try
            {
                var localCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, "505BDC22499CF0F4692CF18D69CDC32F0F285545", true);
                if (localCerts.Count == 0)
                {
                    localCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, "‎505BDC22499CF0F4692CF18D69CDC32F0F285545", false);
                    if (localCerts.Count == 0)
                        Console.WriteLine("Not found.");
                    else
                    {
                        Console.WriteLine("Invlid.");
                    }
                }
                else
                {
                    Console.WriteLine("Found");
                }
                //Console.WriteLine(
                //    certStore.Certificates.Find(X509FindType.FindByThumbprint, "‎‎505BDC22499CF0F4692CF18D69CDC32F0F285545", false)[0]?
                //        .Subject);
                //Console.WriteLine(
                //    certStore.Certificates.Find(X509FindType.FindByThumbprint, "505BDC22499CF0F4692CF18D69CDC32F0F285545", true)[0]?
                //        .Subject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //{
            //    if (args[0] == "--server")
            //    {
            //        var rootCertificate = new X509Certificate2();
            //        rootCertificate.Import(args[1], args[2],
            //            X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            //        var clientCertificate = new X509Certificate2(args[3]);
            //        certStore.Add(rootCertificate);
            //        certStore.Add(clientCertificate);
            //    }
            //    else
            //    {
            //        var clientCertificate = new X509Certificate2();
            //        clientCertificate.Import(args[0], args[1],
            //            X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            //        var rootCertificate = new X509Certificate2(args[2]);
            //        certStore.Add(rootCertificate);
            //        certStore.Add(clientCertificate);
            //    }
            //}
            certStore.Close();
            Console.ReadKey();
            //EventLog.CreateEventSource("ExportService", "Application");
        }
    }
}