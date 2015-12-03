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
        public X509Certificate2 CurrentCert => Server ? RootCert : ClientCert;
        public bool Server { get; }
        public X509Certificate2 ClientCert { get; }
        public X509Certificate2 RootCert { get; }
#if DNX451 || NET451
        private readonly RSACryptoServiceProvider _rootProvider;
        private readonly RSACryptoServiceProvider _clientProvider;
#else
        private readonly RSA _rootProvider;
        private readonly RSA _clientProvider;
#endif
        private readonly Aes _localAes;
        private readonly Dictionary<Guid, SessionInfo> _sessions = new Dictionary<Guid, SessionInfo>();
        private readonly ManualResetEvent _disposeEvent;

        /// <summary>
        /// Server Mode
        /// </summary>
        public ObjectEncryptionHost(bool server, IOptions<AppOptions> options)
        {
            Server = server;
#if NET451 || DNX451
            var certStore = new X509Store(StoreLocation.LocalMachine);
#else
            var certStore = new X509Store();
#endif
            certStore.Open(OpenFlags.ReadOnly);
            var certificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, options.Value.ExportService.ServerCertThumbprint, false);
            if (certificates.Count != 1)
                throw new InvalidOperationException(
                    "Cannot find valid <VC Root> Certificate in Store or more than one certificate with the same subject name installed");

            var rootCert = certificates[0];
            RootCert = new X509Certificate2(rootCert.Export(X509ContentType.Cert));
            if (server)
            {
                if (!rootCert.HasPrivateKey)
                    throw new InvalidOperationException("Root certificate has no private key imported.");
#if DNX451 || NET451
                _rootProvider = (RSACryptoServiceProvider) rootCert.PrivateKey;
#else
                _rootProvider = rootCert.GetRSAPrivateKey();
#endif
                _disposeEvent = new ManualResetEvent(false);
                new Thread(SessionExpirationLookup).Start();
            }
            else
            {
                certificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, options.Value.ExportService.ClientCertThumbprint,
                    false);
                if (certificates.Count != 1)
                    throw new InvalidOperationException(
                        "Cannot find <staging-vc.cloudapp.net> Certificate in Store or more than one certificate with the same subject name installed");
                var clientCert = certificates[0];
                var valid = ValidateClientCertificate(rootCert, clientCert);
                if (!valid)
                {
                    throw new InvalidOperationException("Client certificate is invalid");
                }
                if (!clientCert.HasPrivateKey)
                    throw new InvalidOperationException("Client certificate has no private key imported.");
#if DNX451 || NET451
                _rootProvider = (RSACryptoServiceProvider) rootCert.PublicKey.Key;
                _clientProvider = (RSACryptoServiceProvider) clientCert.PrivateKey;
#else
                _clientProvider = clientCert.GetRSAPrivateKey();
                _rootProvider = rootCert.GetRSAPublicKey();
#endif
                ClientCert = new X509Certificate2(clientCert.Export(X509ContentType.Cert));
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
            var pk = Server ? _rootProvider.ExportParameters(true) : _clientProvider.ExportParameters(true);
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
            var pk = Server ? _rootProvider.ExportParameters(false) : _clientProvider.ExportParameters(false);
            using (var memory = new MemoryStream())
            {
                memory.Write(pk.Exponent, 0, pk.Exponent.Length);
                memory.Write(pk.Modulus, 0, pk.Modulus.Length);
                memory.Seek(0, SeekOrigin.Begin);
                return sha.ComputeHash(memory);
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
                if (Server)
                {
                    serverCert = obj.Certificate;
                    clientCert = ClientCert;
                }
                else
                {
                    serverCert = RootCert;
                    clientCert = obj.Certificate;
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
                var signProvider = Server ? _rootProvider : _clientProvider;
                serializer.Serialize(obj, memory);
                result.Data = memory.ToArray();
                memory.Seek(0, SeekOrigin.Begin);
#if NET451 || DNX451
                result.Sign = signProvider.SignData(memory, new SHA256CryptoServiceProvider());
#else
                result.Sign = signProvider.SignData(memory, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
#endif
            }
            result.Certificate = CurrentCert;
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
            if (!Server)
                throw new InvalidOperationException("Client cannot register session, please use CreateSession");
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
            if (Server)
                throw new InvalidOperationException("Server cannot get/reset client sessions");
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
            if (Server)
                throw new InvalidOperationException("Server cannot create client sessions");
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
            _rootProvider.Dispose();
            _clientProvider.Dispose();
            lock (_sessions)
            {
                foreach (var session in _sessions)
                {
                    session.Value.Dispose();
                }
                _sessions.Clear();
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