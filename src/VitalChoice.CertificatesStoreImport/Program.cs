using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace VitalChoice.CertificatesStoreImport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var certStore = new X509Store("VCCertStore", StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.MaxAllowed);
            if (args[0] == "--server")
            {
                var rootCertificate = new X509Certificate2();
                rootCertificate.Import(args[1], args[2],
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);
                var clientCertificate = new X509Certificate2(args[3]);
                certStore.Add(rootCertificate);
                certStore.Add(clientCertificate);
            }
            else
            {
                var clientCertificate = new X509Certificate2();
                clientCertificate.Import(args[0], args[1],
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);
                var rootCertificate = new X509Certificate2(args[2]);
                certStore.Add(rootCertificate);
                certStore.Add(clientCertificate);
            }
            certStore.Close();
        }
    }
}
