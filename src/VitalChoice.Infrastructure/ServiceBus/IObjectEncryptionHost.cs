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
        T RsaDecrypt<T>(byte[] data, RSACryptoServiceProvider rsa);
        byte[] RsaEncrypt(object obj, RSACryptoServiceProvider rsa);
#else
        T RsaDecrypt<T>(byte[] data, RSA rsa);
        byte[] RsaEncrypt(object obj, RSA rsa);
#endif
        bool RsaCheckSignWithConvert<T>(PlainCommandData obj, out T result);
        PlainCommandData RsaSignWithConvert(object obj);
        T AesDecrypt<T>(byte[] data, Guid session);
        byte[] AesEncrypt(object obj, Guid session);
        bool SessionExist(Guid session);
        bool RegisterSession(Guid session, KeyExchange key);
        KeyExchange GetSessionWithReset(Guid session);
        KeyExchange CreateSession(Guid session);
        bool RemoveSession(Guid session);
        event SessionExpiredEventHandler OnSessionExpired;
        void Dispose();
    }
}