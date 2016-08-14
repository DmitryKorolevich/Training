using System;

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    public interface ISignCheckProvider : IDisposable
    {
        bool VerifyData(byte[] data, byte[] sign);
    }

    public interface ISignProvider : ISignCheckProvider
    {
        byte[] GetPrivateKey();
        byte[] SignData(byte[] data);
    }
}