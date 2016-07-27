﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public delegate void SessionExpiredEventHandler(Guid session, string hostName);

    public class ObjectEncryptionHost : IObjectEncryptionHost
    {
        protected virtual int MaxSessions => 10;
        private readonly string _localEncryptionPath;
        private readonly ILogger _logger;
        private readonly Random _rnd;
        
#if !NETSTANDARD1_5
        private readonly RSACryptoServiceProvider _signProvider;
#else
        private readonly RSA _signProvider;
#endif
        private readonly Aes _localAes;
        private readonly Dictionary<Guid, SessionInfo> _sessions = new Dictionary<Guid, SessionInfo>();
        private readonly ManualResetEvent _disposeEvent;
        private readonly Thread _sessionExpiration;

        /// <summary>
        /// Server Mode
        /// </summary>
        public ObjectEncryptionHost(IOptions<AppOptions> options, ILogger logger)
        {
            _rnd = new Random();
            _localEncryptionPath = options.Value.LocalEncryptionKeyPath;
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
                    _sessionExpiration = new Thread(SessionExpirationLookup);
                    _sessionExpiration.Start();
                }
                _localAes = GetLocalAes();
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message, e);
            }
        }

        public X509Certificate2 RootCert { get; }

        public void UpdateLocalKey(KeyExchange key)
        {
            File.WriteAllBytes(_localEncryptionPath, RsaEncrypt(key.ToCombined(), _signProvider));
            _localAes.Key = key.Key;
            _localAes.IV = key.IV;
        }

        public KeyExchange GetLocalKey()
        {
            return new KeyExchange(_localAes.Key, _localAes.IV);
        }

        public X509Certificate2 LocalCert { get; }

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

#if !NETSTANDARD1_5
        public byte[] RsaDecrypt(byte[] data, RSACryptoServiceProvider rsa)
#else
        public byte[] RsaDecrypt(byte[] data, RSA rsa)
#endif
        {
            if (data == null || rsa == null)
                return null;
#if !NETSTANDARD1_5
            var objectData = rsa.Decrypt(data, true);
#else
            var objectData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA512);
#endif
            return objectData;
        }

#if !NETSTANDARD1_5
        public byte[] RsaEncrypt(byte[] data, RSACryptoServiceProvider rsa)
#else
        public byte[] RsaEncrypt(byte[] data, RSA rsa)
#endif
        {
            if (data == null || rsa == null)
                return null;
#if !NETSTANDARD1_5
            return rsa.Encrypt(data, true);
#else
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);
#endif
        }


        public bool RsaVerifyWithConvert<T>(TransportCommandData command, out T result)
            where T : ServiceBusCommandBase
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command.Data?.Length > 0)
            {
                result = (T) ObjectSerializer.Deserialize(command.Data);
                if (RsaVerifyCommandSign(command))
                {
                    return true;
                }
                return false;
            }
            result = default(T);
            return true;
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
                    _logger.LogError(e.ToString());
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
                _logger.LogError(e.ToString());
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

        public bool IsAuthenticated(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo info;
                if (_sessions.TryGetValue(session, out info))
                {
                    return info.Authenticated;
                }
            }
            return false;
        }

        public void SetAuthenticated(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo info;
                if (_sessions.TryGetValue(session, out info))
                {
                    info.Authenticated = true;
                }
            }
        }

        public bool RegisterSession(Guid session, string hostName, KeyExchange keyExchange)
        {
            Aes aes = Aes.Create();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.KeySize = 256;
            aes.Key = keyExchange.Key;
            aes.IV = keyExchange.IV;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo
            {
                Aes = aes,
                HostName = hostName
            };
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session))
                    return false;
                _sessions.Add(session, encryption);
            }
            return true;
        }

        private void RegisterSession(Guid session, KeyExchange keyCombined)
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
                Aes = aes
            };
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session))
                {
                    _sessions[session] = encryption;
                }
                else
                {
                    _sessions.Add(session, encryption);
                }
            }
        }

        private KeyExchange CreateSession(Guid session)
        {
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
                return new KeyExchange(aes.Key, aes.IV);
            }
        }

        public void RemoveSession(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo sessionInfo;
                if (_sessions.TryGetValue(session, out sessionInfo))
                {
                    _sessions.Remove(session);
                    sessionInfo.Dispose();
                }
            }
        }

        public event SessionExpiredEventHandler OnSessionExpired;

        public void Dispose()
        {
            _disposeEvent?.Set();
            _sessionExpiration?.Abort();
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

#if !NETSTANDARD1_5
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
                    var certs = certStore.Certificates.Cast<X509Certificate2>();
                    _logger.LogWarning(storeName + "\n" + string.Join("\n", certs.Select(c => $"{c.SubjectName.Name}:[{c.Thumbprint}]")));
                    return null;
                }
                var cert = localCerts[0];
                if (signProvider != null)
                {
                    if (!cert.HasPrivateKey)
                        throw new InvalidOperationException("Certificate has no private key imported.");
#if !NETSTANDARD1_5
                    signProvider.ImportParameters(((RSACryptoServiceProvider) cert.PrivateKey).ExportParameters(true));
#else
                    signProvider = cert.GetRSAPrivateKey();
#endif
                }

                return new X509Certificate2(cert.Export(X509ContentType.Cert));
            }
            finally
            {
#if !NETSTANDARD1_5
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
                    OnSessionExpired?.Invoke(expiredSession.Key, expiredSession.Value.HostName);
                    RemoveSession(expiredSession.Key);
                }
            }
        }

        private Aes GetLocalAes()
        {
            var aes = Aes.Create();
            if (aes != null)
            {
                if (!string.IsNullOrWhiteSpace(_localEncryptionPath))
                {
                    KeyExchange exchange;
#if !NETSTANDARD1_5
                    if (!File.Exists(_localEncryptionPath))
                    {
                        var directory = Path.GetDirectoryName(Path.GetFullPath(_localEncryptionPath));
                        if (!string.IsNullOrEmpty(directory))
                        {
                            var security = new DirectorySecurity();
                            var currentUser = WindowsIdentity.GetCurrent().User;
                            if (currentUser == null)
                                throw new AccessDeniedException("Cannot get current User");
                            security.SetOwner(currentUser);
                            security.AddAccessRule(new FileSystemAccessRule(currentUser, FileSystemRights.Modify,
                                AccessControlType.Allow));
                            Directory.CreateDirectory(directory, security);
                        }
                        exchange = new KeyExchange(GetPrivateKeyHash(), Take(GetPublicKeyHash(), 16));
                        File.WriteAllBytes(_localEncryptionPath, RsaEncrypt(exchange.ToCombined(), _signProvider));
                    }
                    else
                    {
                        var key = File.ReadAllBytes(_localEncryptionPath);
                        var combined = RsaDecrypt(key, _signProvider);
                        exchange = new KeyExchange(combined);
                    }
#else
                    var key = File.ReadAllBytes(_localEncryptionPath);
                    var combined = RsaDecrypt(key, _signProvider);
                    exchange = new KeyExchange(combined);
#endif
                    aes.KeySize = 256;
                    aes.IV = exchange.IV;
                    aes.Key = exchange.Key;
                    aes.Padding = PaddingMode.PKCS7;
                }
                else
                {
                    aes.KeySize = 256;
                    aes.IV = Take(GetPublicKeyHash(), 16);
                    aes.Key = GetPrivateKeyHash();
                    aes.Padding = PaddingMode.PKCS7;
                }
            }
            return aes;
        }

        private bool RsaVerifyCommandSign(TransportCommandData commandData)
        {
            if (commandData == null)
                throw new ArgumentNullException(nameof(commandData));

#if !NETSTANDARD1_5
            if (commandData.Certificate?.PublicKey.Key == null || commandData.Sign == null)
#else
            if (commandData.Certificate == null || commandData.Sign == null)
#endif
            {
                _logger.LogWarning("Incoming signature is invalid");
                return false;
            }
#if !NETSTANDARD1_5
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
#if !NETSTANDARD1_5
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
#if !NETSTANDARD1_5
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
#if !NETSTANDARD1_5
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
#if !NETSTANDARD1_5
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

        public Guid GetSession()
        {
            lock (_sessions)
            {
                var chooseSession = _rnd.Next(0, MaxSessions);
                if (_sessions.Count < chooseSession + 1)
                {
                    var newSession = Guid.NewGuid();
                    var keys = CreateSession(newSession);
                    RegisterSession(newSession, keys);
                    return newSession;
                }
                return _sessions.Keys.Skip(chooseSession).FirstOrDefault();
            }
        }

        public KeyExchange GetSessionKeys(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo sessionInfo;
                if (_sessions.TryGetValue(session, out sessionInfo))
                {
                    return new KeyExchange(sessionInfo.Aes.Key, sessionInfo.Aes.IV);
                }
                return null;
            }
        }

        public async Task LockSession(Guid session)
        {
            SessionInfo sessionInfo;
            bool getResult;
            lock (_sessions)
            {
                getResult = _sessions.TryGetValue(session, out sessionInfo);
            }
            if (getResult)
            {
                await sessionInfo.SemaphoreSlim.WaitAsync();
            }
        }

        public void UnlockSession(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo sessionInfo;
                if (_sessions.TryGetValue(session, out sessionInfo))
                {
                    sessionInfo.SemaphoreSlim.Release();
                }
            }
        }

        private class SessionInfo : IDisposable
        {
            public SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1);
            public bool Authenticated { get; set; }
            public Aes Aes { get; set; }
            public DateTime Expiration { get; } = DateTime.Now.AddHours(1);
            public string HostName { get; set; }

            public void Dispose()
            {
                SemaphoreSlim.Dispose();
                Aes?.Dispose();
            }
        }
    }
}