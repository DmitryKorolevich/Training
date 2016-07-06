using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IObjectEncryptionHost
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
        bool SessionExist(Guid session);
        bool RegisterSession(Guid session, string hostName, KeyExchange keyCombined);
        bool RegisterSession(Guid session, KeyExchange keyCombined);
        KeyExchange CreateSession(Guid session);
        KeyExchange GetSession(Guid session);
        bool RemoveSession(Guid session);
        event SessionExpiredEventHandler OnSessionExpired;
        void Dispose();
    }
}