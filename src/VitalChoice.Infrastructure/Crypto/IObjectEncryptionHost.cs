using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;

namespace VitalChoice.Infrastructure.Crypto
{
    public interface IObjectEncryptionHost : IDisposable
    {
        byte[] HashBytes(byte[] data);
        string HashString(string data);
        void UpdateLocalKey(KeyExchange key);
        KeyExchange GetLocalKey();
        X509Certificate2 LocalCert { get; }
        T LocalDecrypt<T>(byte[] data);
        byte[] LocalEncrypt(object obj);
        byte[] RsaDecrypt(byte[] data, RSACng rsa);
        byte[] RsaEncrypt(byte[] data, RSACng rsa);
        bool VerifySignWithConvert<T>(TransportCommandData command, out T result)
            where T : ServiceBusCommandBase;
        TransportCommandData SignWithConvert(ServiceBusCommandBase command);

        T AesDecryptVerify<T>(TransportCommandData command, Guid session)
            where T : ServiceBusCommandBase;

        TransportCommandData AesEncryptSign(ServiceBusCommandBase command, Guid session);
        Guid GetSession();
        bool SessionExist(Guid session);
        void RemoveSession(Guid session);
        bool RegisterSession(Guid session, string hostName, KeyExchange keyCombined);
        KeyExchange GetSessionKeys(Guid session);

        event SessionExpiredEventHandler OnSessionExpired;
    }
}