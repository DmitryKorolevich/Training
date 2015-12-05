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
#if NET451 || NET451
        byte[] RsaDecrypt(byte[] data, RSACryptoServiceProvider rsa);
        byte[] RsaEncrypt(byte[] data, RSACryptoServiceProvider rsa);
#else
        byte[] RsaDecrypt(byte[] data, RSA rsa);
        byte[] RsaEncrypt(byte[] data, RSA rsa);
#endif
        bool RsaCheckSignWithConvert<T>(PlainCommandData obj, out T result);
        PlainCommandData RsaSignWithConvert(object obj);
        T AesDecrypt<T>(SymmetricEncryptedCommandData data, Guid session);
        SymmetricEncryptedCommandData AesEncrypt(object obj, Guid session);
        bool SessionExist(Guid session);
        bool RegisterSession(Guid session, string hostName, byte[] keyCombined);
        KeyExchange GetSessionWithReset(Guid session);
        KeyExchange CreateSession(Guid session);
        bool RemoveSession(Guid session);
        event SessionExpiredEventHandler OnSessionExpired;
        void Dispose();
    }
}