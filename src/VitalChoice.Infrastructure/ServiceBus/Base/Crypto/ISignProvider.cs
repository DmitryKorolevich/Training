using System;

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    public interface ISignProvider : IDisposable
    {
        byte[] GetPrivateKey();
        bool VerifyData(byte[] data, byte[] sign);
        byte[] SignData(byte[] data);
    }
}