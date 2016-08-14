using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    public interface IObjectEncryptionHost : IDisposable
    {
        void UpdateLocalKey(KeyExchange key);
        KeyExchange GetLocalKey();
        X509Certificate2 LocalCert { get; }
        T LocalDecrypt<T>(byte[] data);
        byte[] LocalEncrypt(object obj);
        bool ValidateClientCertificate(X509Certificate2 clientCert);
        byte[] RsaDecrypt(byte[] data, RSACng rsa);
        byte[] RsaEncrypt(byte[] data, RSACng rsa);
        bool VerifySignWithConvert<T>(TransportCommandData command, out T result)
            where T : ServiceBusCommandBase;
        TransportCommandData SignWithConvert(ServiceBusCommandBase command);

        T AesDecryptVerify<T>(TransportCommandData data, Guid session)
            where T : ServiceBusCommandBase;

        TransportCommandData AesEncryptSign(ServiceBusCommandBase command, Guid session);
        Guid GetSession();
        bool SessionExist(Guid session);
        void SetAuthenticated(Guid session);
        bool IsAuthenticated(Guid session);
        void RemoveSession(Guid session);
        bool RegisterSession(Guid session, string hostName, KeyExchange keyCombined);
        KeyExchange GetSessionKeys(Guid session);

        Task LockSession(Guid session);

        void UnlockSession(Guid session);

        event SessionExpiredEventHandler OnSessionExpired;
    }
}