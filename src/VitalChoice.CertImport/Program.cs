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
            //var certStore = new X509Store(StoreLocation.LocalMachine);
            //certStore.Open(OpenFlags.MaxAllowed);
            //if (certStore.Certificates.Find(X509FindType.FindBySubjectName, "CN=VC Root", false).Count == 0)
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
            //certStore.Close();
            EventLog.CreateEventSource("ExportService", "Application");
        }
    }
}