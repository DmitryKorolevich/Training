using System.Security.Cryptography.X509Certificates;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public class PlainCommandData
    {
        public X509Certificate2 Certificate { get; set; }

        public byte[] Data { get; set; }
    }

    public class KeyExchange
    {
        public byte[] Key { get; set; }

        public byte[] IV { get; set; }
    }
}
