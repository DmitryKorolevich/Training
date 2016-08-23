using System;
using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts
{
    [DataContract]
    public class TransportCommandData
    {
        public TransportCommandData(byte[] data)
        {
            Data = data;
        }

        [DataMember]
        public string CertThumbprint { get; set; }

        [DataMember]
        public byte[] Data { get; set; }

        [DataMember]
        public byte[] Sign { get; set; }
    }

    [DataContract]
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

        [DataMember]
        public byte[] Key { get; set; }

        [DataMember]
        public byte[] IV { get; set; }

        public byte[] ToCombined()
        {
            var keyCombined = new byte[IV.Length + Key.Length];
            Array.Copy(IV, keyCombined, IV.Length);
            Array.Copy(Key, 0, keyCombined, IV.Length, Key.Length);
            return keyCombined;
        }
    }
}