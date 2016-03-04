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
        public KeyExchange()
        {
            Key = new byte[32];
            IV = new byte[16];
        }

        public KeyExchange(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public KeyExchange(byte[] keyCombined)
        {
            Key = new byte[32];
            IV = new byte[16];
            Array.Copy(keyCombined, IV, 16);
            Array.Copy(keyCombined, 16, Key, 0, 32);
        }

        public byte[] Key { get; }

        public byte[] IV { get; }

        public byte[] ToCombined()
        {
            var keyCombined = new byte[IV.Length + Key.Length];
            Array.Copy(IV, keyCombined, IV.Length);
            Array.Copy(Key, 0, keyCombined, IV.Length, Key.Length);
            return keyCombined;
        }
    }
}