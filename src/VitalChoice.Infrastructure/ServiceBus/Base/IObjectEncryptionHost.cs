using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IObjectEncryptionHost : IDisposable
    {
        void UpdateLocalKey(KeyExchange key);
        KeyExchange GetLocalKey();
        X509Certificate2 LocalCert { get; }
        T LocalDecrypt<T>(byte[] data);
        byte[] LocalEncrypt(object obj);
        bool ValidateClientCertificate(X509Certificate2 clientCert);
#if !NETSTANDARD1_5
        byte[] RsaDecrypt(byte[] data, RSACryptoServiceProvider rsa);
        byte[] RsaEncrypt(byte[] data, RSACryptoServiceProvider rsa);
#else
        byte[] RsaDecrypt(byte[] data, RSA rsa);
        byte[] RsaEncrypt(byte[] data, RSA rsa);
#endif

        bool RsaVerifyWithConvert<T>(TransportCommandData command, out T result)
            where T : ServiceBusCommandBase;

        TransportCommandData RsaSignWithConvert(ServiceBusCommandBase command);

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
        void Dispose();
    }
}