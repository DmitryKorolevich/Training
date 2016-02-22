using System;
using System.Security.Cryptography.X509Certificates;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
#if NET451
    [Serializable]
#endif
    public class TransportCommandData
    {
        public TransportCommandData(byte[] data)
        {
            Data = data;
        }

        public X509Certificate2 Certificate { get; set; }

        public byte[] Data { get; set; }

        public byte[] Sign { get; set; }
    }

#if NET451
    [Serializable]
#endif
    public class KeyExchange
    {
        public byte[] Key { get; set; }

        public byte[] IV { get; set; }
    }
}