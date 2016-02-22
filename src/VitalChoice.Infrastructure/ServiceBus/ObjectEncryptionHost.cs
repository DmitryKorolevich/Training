using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public delegate void SessionExpiredEventHandler(Guid session, string hostName);

    public class ObjectEncryptionHost : IDisposable, IObjectEncryptionHost
    {
        private readonly ILogger _logger;
        public X509Certificate2 RootCert { get; }
        public X509Certificate2 LocalCert { get; }
#if NET451
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
        public ObjectEncryptionHost(IOptions<AppOptions> options, ILogger logger)
        {
            _logger = logger;
            try
            {
                _signProvider = new RSACryptoServiceProvider();
                LocalCert = GetPublicCertificate(StoreName.My, options.Value.ExportService.CertThumbprint, _signProvider);
                if (LocalCert == null)
                {
                    _signProvider.Dispose();
                    _signProvider = null;
                }

                RootCert = GetPublicCertificate(StoreName.Root, options.Value.ExportService.RootThumbprint);
                if (RootCert == null)
                    return;

                if (!ValidateClientCertificate(LocalCert))
                {
                    throw new InvalidOperationException("Client and/or root certificate(s) doesn't valid.");
                }
                if (options.Value.ExportService.EncryptionHostSessionExpire)
                {
                    _disposeEvent = new ManualResetEvent(false);
                    new Thread(SessionExpirationLookup).Start();
                    _sessionExpires = true;
                }
                _localAes = GetLocalAes();
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
        }

        public T LocalDecrypt<T>(byte[] data)
        {
            using (var memory = new MemoryStream())
            {
                using (var decryptor = _localAes.CreateDecryptor())
                {

                    using (CryptoStream cs = new CryptoStream(memory, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        return (T) ObjectSerializer.Deserialize(memory.ToArray());
                    }
                }
            }
        }

        public byte[] LocalEncrypt(object obj)
        {
            var plainData = ObjectSerializer.Serialize(obj);
            using (var memory = new MemoryStream())
            {
                using (var encryptor = _localAes.CreateEncryptor())
                {
                    using (CryptoStream cs = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainData, 0, plainData.Length);
                        cs.FlushFinalBlock();
                        return memory.ToArray();
                    }
                }
            }
        }

        public bool ValidateClientCertificate(X509Certificate2 clientCert)
        {
            if (clientCert == null)
                throw new ArgumentNullException(nameof(clientCert));

            X509Chain validationChain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                    RevocationFlag = X509RevocationFlag.EntireChain,
                    VerificationFlags = X509VerificationFlags.NoFlag,
                    VerificationTime = DateTime.Now,
                    UrlRetrievalTimeout = TimeSpan.Zero
                }
            };
            validationChain.ChainPolicy.ExtraStore.Add(RootCert);
            var valid = validationChain.Build(clientCert);
            return valid;
        }

#if NET451
        public byte[] RsaDecrypt(byte[] data, RSACryptoServiceProvider rsa)
#else
        public byte[] RsaDecrypt(byte[] data, RSA rsa)
#endif
        {
            if (data == null || rsa == null)
                return null;
#if NET451
            var objectData = rsa.Decrypt(data, true);
#else
            var objectData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA512);
#endif
            return objectData;
        }

#if NET451
        public byte[] RsaEncrypt(byte[] data, RSACryptoServiceProvider rsa)
#else
        public byte[] RsaEncrypt(byte[] data, RSA rsa)
#endif
        {
            if (data == null || rsa == null)
                return null;
#if NET451
            return rsa.Encrypt(data, true);
#else
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);
#endif
        }


        public bool RsaVerifyWithConvert<T>(TransportCommandData command, out T result)
            where T: ServiceBusCommandBase
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.Data?.Length > 0)
            {
                if (RsaVerifyCommandSign(command))
                {
                    result = (T) ObjectSerializer.Deserialize(command.Data);
                    return true;
                }
            }
            result = default(T);
            return false;
        }

        public TransportCommandData RsaSignWithConvert(ServiceBusCommandBase command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new TransportCommandData(ObjectSerializer.Serialize(command));
            RsaSignCommand(result);
            return result;
        }

        public T AesDecryptVerify<T>(TransportCommandData data, Guid session)
            where T: ServiceBusCommandBase
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Data == null)
                return default(T);
            SessionInfo encryption;
            lock (_sessions)
            {
                if (!_sessions.TryGetValue(session, out encryption))
                    return default(T);
            }
            if (RsaVerifyCommandSign(data))
            {
                encryption.Expiration = DateTime.Now.AddHours(1);
                byte[] plainData = null;
                try
                {
                    using (var memory = new MemoryStream())
                    {
                        using (var decryptor = encryption.Aes.CreateDecryptor())
                        {
                            using (CryptoStream cs = new CryptoStream(memory, decryptor, CryptoStreamMode.Write))
                            {
                                cs.Write(data.Data, 0, data.Data.Length);
                                cs.FlushFinalBlock();
                                plainData = memory.ToArray();
                                return (T) ObjectSerializer.Deserialize(plainData);
                            }
                        }
                    }
                }
                catch (SerializationException)
                {
                    if (plainData != null)
                    {
                        _logger.LogError($"Cannot deserialize object \r\n{Encoding.UTF8.GetString(plainData)}");
                    }
                }
                catch (CryptographicException e)
                {
                    _logger.LogError(e.Message, e);
                }
            }
            return default(T);
        }

        public TransportCommandData AesEncryptSign(ServiceBusCommandBase command, Guid session)
        {
            if (command == null)
                return new TransportCommandData(null);
            SessionInfo encryption;
            lock (_sessions)
            {
                if (!_sessions.TryGetValue(session, out encryption))
                {
                    _logger.LogWarning($"Session doesn't exist {session}");
                    return new TransportCommandData(null);
                }
            }
            encryption.Expiration = DateTime.Now.AddHours(1);
            try
            {
                using (var encryptor = encryption.Aes.CreateEncryptor())
                {
                    var plainData = ObjectSerializer.Serialize(command);
                    using (var memory = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(plainData, 0, plainData.Length);
                            cs.FlushFinalBlock();
                            var encryptedData = memory.ToArray();
                            var result = new TransportCommandData(encryptedData);
                            RsaSignCommand(result);
                            return result;
                        }
                    }
                }
            }
            catch (CryptographicException e)
            {
                _logger.LogError(e.Message, e);
            }
            catch (SerializationException)
            {
                _logger.LogError($"Cannot serialize object {command}");
            }
            return new TransportCommandData(null);
        }

        public bool SessionExist(Guid session)
        {
            lock (_sessions)
            {
                return _sessions.ContainsKey(session);
            }
        }

        public bool RegisterSession(Guid session, string hostName, byte[] keyCombined)
        {
            Aes aes = Aes.Create();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.KeySize = 256;
            var key = new byte[32];
            var iv = new byte[16];
            Array.Copy(keyCombined, iv, 16);
            Array.Copy(keyCombined, 16, key, 0, 32);
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo
            {
                Aes = aes,
                Expiration = DateTime.Now.AddHours(1),
                HostName = hostName
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

        public bool RegisterSession(Guid session, KeyExchange keyCombined)
        {
            Aes aes = Aes.Create();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.KeySize = 256;
            aes.Key = keyCombined.Key;
            aes.IV = keyCombined.IV;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo
            {
                Aes = aes,
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
                aes.Padding = PaddingMode.PKCS7;
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
            _signProvider?.Dispose();
            lock (_sessions)
            {
                foreach (var session in _sessions)
                {
                    session.Value.Dispose();
                }
                _sessions.Clear();
            }
        }

#if NET451
        private X509Certificate2 GetPublicCertificate(StoreName storeName, string thumbprint, RSACryptoServiceProvider signProvider = null)
#else
        private X509Certificate2 GetPublicCertificate(StoreName storeName, string thumbprint, RSA signProvider = null)
#endif
        {
            var certStore = new X509Store(storeName, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            try
            {
                var localCerts = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true);
                if (localCerts.Count == 0)
                {
                    _logger.LogCritical(
                        $"Cannot find Certificate in Store <{certStore.Name}:{certStore.Location}> with thumbprint <{thumbprint}>");
                    return null;
                }
                var cert = localCerts[0];
                if (signProvider != null)
                {
                    if (!cert.HasPrivateKey)
                        throw new InvalidOperationException("Certificate has no private key imported.");
#if NET451
                    signProvider.ImportParameters(((RSACryptoServiceProvider) cert.PrivateKey).ExportParameters(true));
#else
                    signProvider = cert.GetRSAPrivateKey();
#endif
                }

                return new X509Certificate2(cert.Export(X509ContentType.Cert));
            }
            finally
            {
#if NET451
                certStore.Close();
#else
                certStore.Dispose();
#endif
            }
        }

        private void SessionExpirationLookup()
        {
            while (!_disposeEvent.WaitOne(new TimeSpan(0, 10, 0)))
            {
                KeyValuePair<Guid, SessionInfo>[] expiredSessions;
                lock (_sessions)
                {
                    expiredSessions =
                        _sessions.Where(session => session.Value.Expiration <= DateTime.Now).ToArray();
                }
                foreach (var expiredSession in expiredSessions)
                {
                    RemoveSession(expiredSession.Key);
                    OnSessionExpired?.Invoke(expiredSession.Key, expiredSession.Value.HostName);
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
                aes.Padding = PaddingMode.PKCS7;
            }
            return aes;
        }

        private bool RsaVerifyCommandSign(TransportCommandData commandData)
        {
            if (commandData == null)
                throw new ArgumentNullException(nameof(commandData));
#if NET451
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(((RSACryptoServiceProvider)commandData.Certificate.PublicKey.Key).ExportParameters(false));
#else
            var rsa = commandData.Certificate.GetRSAPublicKey();
#endif
            if (!ValidateClientCertificate(commandData.Certificate))
            {
                _logger.LogWarning(
                    $"Invalid certificate:\n Server Thumbprint: {RootCert.Thumbprint}\n Client Thumbprint: {commandData.Certificate.Thumbprint}");
                return false;
            }
#if NET451
            if (!rsa.VerifyData(commandData.Data, CryptoConfig.MapNameToOID("SHA512"), commandData.Sign))
#else
            if (!rsa.VerifyData(commandData.Data, commandData.Sign, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1))
#endif
            {
                _logger.LogWarning($"Invalid sign. Remote Certificate Thumbprint: {commandData.Certificate.Thumbprint}, ");
                return false;
            }
            return true;
        }

        private void RsaSignCommand(TransportCommandData commandData)
        {
            if (_signProvider == null)
                return;
#if NET451
            commandData.Sign = _signProvider.SignData(commandData.Data, CryptoConfig.MapNameToOID("SHA512"));
#else
            commandData.Sign = _signProvider.SignData(commandData.Data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
#endif
            commandData.Certificate = LocalCert;
        }

        private byte[] Take(byte[] data, int length)
        {
            var result = new byte[length];
            Array.Copy(data, result, length);
            return result;
        }

        private byte[] GetPrivateKeyHash()
        {
#if NET451
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
#if NET451
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
            public Aes Aes { get; set; }
            public DateTime Expiration { get; set; }
            public string HostName { get; set; }

            public void Dispose()
            {
                Aes?.Dispose();
            }
        }
    }
}