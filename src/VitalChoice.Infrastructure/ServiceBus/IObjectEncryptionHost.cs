using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public interface IObjectEncryptionHost
    {
        X509Certificate2 LocalCert { get; }
        T LocalDecrypt<T>(byte[] data);
        byte[] LocalEncrypt(object obj);
        bool ValidateClientCertificate(X509Certificate2 rootCert, X509Certificate2 clientCert);
#if DNX451 || NET451
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
        bool RegisterSession(Guid session, string hostName, byte[] keyCombined);
        KeyExchange GetSessionWithReset(Guid session);
        KeyExchange CreateSession(Guid session);
        bool RemoveSession(Guid session);
        event SessionExpiredEventHandler OnSessionExpired;
        void Dispose();
    }
}