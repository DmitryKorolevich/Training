using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public delegate void SessionExpiredEventHandler(Guid session);

    public class ObjectEncryptionHost : IDisposable, IObjectEncryptionHost
    {
        public X509Certificate2 LocalCert { get; }
#if DNX451 || NET451
        private readonly RSACryptoServiceProvider _signProvider;
#else
        private readonly RSA _signProvider;
#endif
        private readonly Aes _localAes;
        private readonly bool _sessionExpires;
        private readonly Dictionary<Guid, SessionInfo> _sessions = new Dictionary<Guid, SessionInfo>();
        private readonly ManualResetEvent _disposeEvent;

        /// <summary>
        /// Server Mode
        /// </summary>
        public ObjectEncryptionHost(IOptions<AppOptions> options)
        {
#if NET451 || DNX451
            var certStore = new X509Store(StoreLocation.LocalMachine);
#else
            var certStore = new X509Store();
#endif
            certStore.Open(OpenFlags.ReadOnly);
            var certificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, options.Value.ExportService.CertThumbprint, false);
            if (certificates.Count == 0)
                throw new InvalidOperationException(
                    $"Cannot find Certificate in Store with thumbprint <{options.Value.ExportService.CertThumbprint}>");

            var cert = certificates[0];
            LocalCert = new X509Certificate2(cert.Export(X509ContentType.Cert));
                if (!cert.HasPrivateKey)
                    throw new InvalidOperationException("Root certificate has no private key imported.");
#if DNX451 || NET451
                _signProvider = (RSACryptoServiceProvider) cert.PrivateKey;
#else
                _signProvider = cert.GetRSAPrivateKey();
#endif
            if (options.Value.ExportService.EncryptionHostSessionExpire)
            {
                _disposeEvent = new ManualResetEvent(false);
                new Thread(SessionExpirationLookup).Start();
                _sessionExpires = true;
            }
#if DNX451 || NET451
            certStore.Close();
#else
            certStore.Dispose();
#endif
            _localAes = GetLocalAes();
        }

        public T LocalDecrypt<T>(byte[] data)
        {
            using (var memory = new MemoryStream())
            {
                var decryptor = _localAes.CreateDecryptor();

                using (CryptoStream cs = new CryptoStream(memory, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
                memory.Seek(0, SeekOrigin.Begin);
                SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
                return (T) serializer.Deserialize(memory);
            }
        }

        public byte[] LocalEncrypt(object obj)
        {
            byte[] plainData;
            using (var memory = new MemoryStream())
            {
                SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
                serializer.Serialize(obj, memory);
                plainData = memory.ToArray();
            }
            using (var memory = new MemoryStream())
            {
                var encryptor = _localAes.CreateDecryptor();
                using (CryptoStream cs = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainData, 0, plainData.Length);
                }
                return memory.ToArray();
            }
        }

        public bool ValidateClientCertificate(X509Certificate2 rootCert, X509Certificate2 clientCert)
        {
            X509Chain validationChain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                    RevocationFlag = X509RevocationFlag.ExcludeRoot,
                    VerificationFlags = X509VerificationFlags.IgnoreWrongUsage,
                    VerificationTime = DateTime.Now,
                    UrlRetrievalTimeout = TimeSpan.Zero
                }
            };
            validationChain.ChainPolicy.ExtraStore.Add(rootCert);
            var valid = validationChain.Build(clientCert);
            return valid;
        }

#if DNX451 || NET451
        public T RsaDecrypt<T>(byte[] data, RSACryptoServiceProvider rsa)
#else
        public T RsaDecrypt<T>(byte[] data, RSA rsa)
#endif
        {
            if (data == null)
                return default(T);
#if DNX451 || NET451
            var objectData = rsa.Decrypt(data, true);
#else
            var objectData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
#endif
            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream(objectData))
            {
                return (T)serializer.Deserialize(memory);
            }
        }

#if DNX451 || NET451
        public byte[] RsaEncrypt(object obj, RSACryptoServiceProvider rsa)
#else
        public byte[] RsaEncrypt(object obj, RSA rsa)
#endif
        {
            if (obj == null)
                return null;
            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(obj, memory);
#if DNX451 || NET451
                return rsa.Encrypt(memory.ToArray(), true);
#else
                return rsa.Encrypt(memory.ToArray(), RSAEncryptionPadding.OaepSHA256);
#endif
            }
        }

        public bool RsaCheckSignWithConvert<T>(PlainCommandData obj, out T result)
        {
            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream())
            {
                memory.Write(obj.Data, 0, obj.Data.Length);
                memory.Seek(0, SeekOrigin.Begin);
                result = (T) serializer.Deserialize(memory);
#if DNX451 || NET451
                var rsa = (RSACryptoServiceProvider)obj.Certificate.PublicKey.Key;
#else
                var rsa = obj.Certificate.GetRSAPublicKey();
#endif
                X509Certificate2 serverCert, clientCert;
                if (_sessionExpires)
                {
                    clientCert = obj.Certificate;
                    serverCert = LocalCert;
                }
                else
                {
                    serverCert = obj.Certificate;
                    clientCert = LocalCert;
                }
#if DNX451 || NET451
                if (ValidateClientCertificate(serverCert, clientCert) &&
                    rsa.VerifyData(obj.Data, new SHA256CryptoServiceProvider(), obj.Sign))
#else
                if (ValidateClientCertificate(serverCert, clientCert) &&
                    rsa.VerifyData(obj.Data, obj.Sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
#endif

                return true;
                result = default(T);
                return false;
            }
        }

        public PlainCommandData RsaSignWithConvert(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var result = new PlainCommandData();
            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(obj, memory);
                result.Data = memory.ToArray();
                memory.Seek(0, SeekOrigin.Begin);
#if NET451 || DNX451
                result.Sign = _signProvider.SignData(memory, new SHA256CryptoServiceProvider());
#else
                result.Sign = _signProvider.SignData(memory, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
#endif
            }
            result.Certificate = LocalCert;
            return result;
        }

        public T AesDecrypt<T>(byte[] data, Guid session)
        {
            lock (_sessions)
            {
                SessionInfo encryption;
                if (_sessions.TryGetValue(session, out encryption))
                {
                    encryption.Expiration = DateTime.Now.AddHours(1);
                    using (var memory = new MemoryStream())
                    {
                        if (!encryption.Decryptor.CanReuseTransform)
                            encryption.Decryptor = encryption.Aes.CreateDecryptor();

                        using (CryptoStream cs = new CryptoStream(memory, encryption.Decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                        }
                        memory.Seek(0, SeekOrigin.Begin);
                        SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
                        return (T) serializer.Deserialize(memory);
                    }
                }
            }
            return default(T);
        }

        public byte[] AesEncrypt(object obj, Guid session)
        {
            lock (_sessions)
            {
                SessionInfo encryption;
                if (_sessions.TryGetValue(session, out encryption))
                {
                    encryption.Expiration = DateTime.Now.AddHours(1);
                    byte[] plainData;
                    using (var memory = new MemoryStream())
                    {
                        if (!encryption.Encryptor.CanReuseTransform)
                            encryption.Encryptor = encryption.Aes.CreateEncryptor();

                        SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
                        serializer.Serialize(obj, memory);
                        plainData = memory.ToArray();
                    }
                    using (var memory = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(memory, encryption.Encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(plainData, 0, plainData.Length);
                        }
                        return memory.ToArray();
                    }
                }
            }
            return null;
        }

        public bool SessionExist(Guid session)
        {
            lock (_sessions)
            {
                return _sessions.ContainsKey(session);
            }
        }

        public bool RegisterSession(Guid session, KeyExchange key)
        {
            if (!_sessionExpires)
                throw new InvalidOperationException("Cannot register session, as it's state not controlled by session expiration.");
            Aes aes = Aes.Create();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.Key = key.Key;
            aes.IV = key.IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo
            {
                Aes = aes,
                Encryptor = aes.CreateEncryptor(),
                Decryptor = aes.CreateDecryptor(),
                Expiration = DateTime.Now.AddHours(1)
            };
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session))
                    _sessions[session] = encryption;
                else
                    _sessions.Add(session, encryption);
            }
            return true;
        }

        public KeyExchange GetSessionWithReset(Guid session)
        {
            if (_sessionExpires)
                throw new InvalidOperationException("Cannot get/reset client sessions as it's state controlled by expiration.");
            lock (_sessions)
            {
                SessionInfo aes;
                if (_sessions.TryGetValue(session, out aes))
                {
                    aes.Encryptor = aes.Aes.CreateEncryptor();
                    aes.Decryptor = aes.Aes.CreateDecryptor();
                    aes.Expiration = DateTime.Now.AddHours(1);
                    return new KeyExchange
                    {
                        Key = aes.Aes.Key,
                        IV = aes.Aes.IV
                    };
                }
                return null;
            }
        }

        public KeyExchange CreateSession(Guid session)
        {
            if (_sessionExpires)
                throw new InvalidOperationException("Cannot create client sessions as it's state controlled by expiration.");
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session))
                    return null;
                Aes aes = Aes.Create();
                if (aes == null)
                    throw new InvalidOperationException("Cannot initialize AES encryption");
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                SessionInfo encryption = new SessionInfo
                {
                    Aes = aes,
                    Encryptor = aes.CreateEncryptor(),
                    Decryptor = aes.CreateDecryptor(),
                    Expiration = DateTime.Now.AddHours(1)
                };
                _sessions.Add(session, encryption);
                return new KeyExchange
                {
                    Key = aes.Key,
                    IV = aes.IV
                };
            }
        }

        public bool RemoveSession(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo aes;
                if (_sessions.TryGetValue(session, out aes))
                {
                    _sessions.Remove(session);
                    aes.Dispose();
                    return true;
                }
            }
            return false;
        }

        public event SessionExpiredEventHandler OnSessionExpired;

        public void Dispose()
        {
            _disposeEvent?.Dispose();
            _signProvider.Dispose();
            lock (_sessions)
            {
                foreach (var session in _sessions)
                {
                    session.Value.Dispose();
                }
                _sessions.Clear();
            }
        }

        private void SessionExpirationLookup()
        {
            while (!_disposeEvent.WaitOne(new TimeSpan(0, 10, 0)))
            {
                Guid[] expiredSessions;
                lock (_sessions)
                {
                    expiredSessions =
                        _sessions.Where(session => session.Value.Expiration <= DateTime.Now).Select(session => session.Key).ToArray();
                }
                foreach (var expiredSession in expiredSessions)
                {
                    RemoveSession(expiredSession);
                    OnSessionExpired?.Invoke(expiredSession);
                }
            }
        }

        private Aes GetLocalAes()
        {
            var aes = Aes.Create();
            if (aes != null)
            {
                aes.KeySize = 256;
                aes.IV = Take(GetPublicKeyHash(), 16);
                aes.Key = GetPrivateKeyHash();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
            }
            return aes;
        }

        private byte[] Take(byte[] data, int length)
        {
            var result = new byte[length];
            Array.Copy(data, result, length);
            return result;
        }

        private byte[] GetPrivateKeyHash()
        {
#if DNX451 || NET451
            var sha = new SHA256CryptoServiceProvider();
#else
            var sha = SHA256.Create();
#endif
            sha.Initialize();
            var pk = _signProvider.ExportParameters(true);
            using (var memory = new MemoryStream())
            {
                memory.Write(pk.D, 0, pk.D.Length);
                memory.Write(pk.DP, 0, pk.DP.Length);
                memory.Write(pk.DQ, 0, pk.DQ.Length);
                memory.Write(pk.Exponent, 0, pk.Exponent.Length);
                memory.Write(pk.InverseQ, 0, pk.InverseQ.Length);
                memory.Write(pk.Modulus, 0, pk.Modulus.Length);
                memory.Write(pk.P, 0, pk.P.Length);
                memory.Write(pk.Q, 0, pk.Q.Length);
                memory.Seek(0, SeekOrigin.Begin);
                return sha.ComputeHash(memory);
            }
        }

        private byte[] GetPublicKeyHash()
        {
#if DNX451 || NET451
            var sha = new SHA256CryptoServiceProvider();
#else
            var sha = SHA256.Create();
#endif
            sha.Initialize();
            var pk = _signProvider.ExportParameters(false);
            using (var memory = new MemoryStream())
            {
                memory.Write(pk.Exponent, 0, pk.Exponent.Length);
                memory.Write(pk.Modulus, 0, pk.Modulus.Length);
                memory.Seek(0, SeekOrigin.Begin);
                return sha.ComputeHash(memory);
            }
        }

        private struct SessionInfo : IDisposable
        {
            public ICryptoTransform Encryptor { get; set; }
            public ICryptoTransform Decryptor { get; set; }
            public Aes Aes { get; set; }
            public DateTime Expiration { get; set; }

            public void Dispose()
            {
                Encryptor?.Dispose();
                Decryptor?.Dispose();
                Aes?.Dispose();
            }
        }
    }
}