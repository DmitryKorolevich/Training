using System;
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

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    public delegate void SessionExpiredEventHandler(Guid session, string hostName);

    public class ObjectEncryptionHost : IObjectEncryptionHost
    {
        protected static string SignAlgorithmType => "ECDsaCng";

        protected virtual int MaxSessions => 10;
        private readonly string _localEncryptionPath;
        private readonly ILogger _logger;
        private readonly Random _rnd;
        
        private readonly ISignProvider _signProvider;
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
                X509Certificate2 cert;
                _signProvider = GetPublicCertificate(StoreName.My, options.Value.ExportService.CertThumbprint, true, out cert);
                if (cert == null)
                {
                    _signProvider?.Dispose();
                    _signProvider = null;
                }
                else
                {
                    LocalCert = cert;
                }

                GetPublicCertificate(StoreName.Root, options.Value.ExportService.RootThumbprint, false, out cert);
                if (cert == null)
                {
                    return;
                }
                RootCert = cert;

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
            var data = ProtectedData.Protect(key.ToCombined(), _signProvider?.GetPrivateKey(), DataProtectionScope.CurrentUser);
            File.WriteAllBytes(_localEncryptionPath, data);
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

        public byte[] RsaDecrypt(byte[] data, RSACng rsa)
        {
            if (data == null || rsa == null)
                return null;
            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public byte[] RsaEncrypt(byte[] data, RSACng rsa)
        {
            if (data == null || rsa == null)
                return null;
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public bool VerifySignWithConvert<T>(TransportCommandData command, out T result)
            where T : ServiceBusCommandBase
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command.Data?.Length > 0)
            {
                result = (T) ObjectSerializer.Deserialize(command.Data);
                if (VerifyCommandSign(command))
                {
                    return true;
                }
                return false;
            }
            result = default(T);
            return false;
        }

        public TransportCommandData SignWithConvert(ServiceBusCommandBase command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new TransportCommandData(ObjectSerializer.Serialize(command));
            SignCommand(result);
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
            if (VerifyCommandSign(data))
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
                            SignCommand(result);
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
        
        private ISignProvider GetPublicCertificate(StoreName storeName, string thumbprint, bool provideSigning,
            out X509Certificate2 result)
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
                    result = null;
                    return null;
                }
                var cert = localCerts[0];
                result = new X509Certificate2(cert.Export(X509ContentType.Cert));
                if (provideSigning)
                {
                    return GetSignProvider(cert);
                }
                return null;
            }
            finally
            {
#if !NETSTANDARD1_6
                certStore.Close();
#else
                certStore.Dispose();
#endif
            }
        }

        private ISignProvider GetSignProvider(X509Certificate2 cert)
        {
            if (!cert.HasPrivateKey)
                throw new InvalidOperationException("Certificate has no private key imported.");

            switch (SignAlgorithmType)
            {
                case "RSA":
                case "System.Security.Cryptography.RSA":
                    return new RsaSignProvider(cert.GetRSAPrivateKey());
                case "DSA":
                case "System.Security.Cryptography.DSA":
                    return new DsaSignProvider(cert.GetDSAPrivateKey());
                case "ECDsa":
                case "ECDsaCng":
                case "System.Security.Cryptography.ECDsaCng":
                    return new ECDsaSignProvider(cert.GetECDsaPrivateKey());
                default:
                    throw new NotImplementedException($"{SignAlgorithmType} is not supported");
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

        private AesCng GetLocalAes()
        {
            var aes = new AesCng();
            if (!string.IsNullOrWhiteSpace(_localEncryptionPath))
            {
                KeyExchange exchange;
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
                    exchange = new KeyExchange(GetPrivateKeyHash(), Take(GetPrivateKeyHash(), 16));
                    var data = ProtectedData.Protect(exchange.ToCombined(), _signProvider?.GetPrivateKey(),
                        DataProtectionScope.CurrentUser);
                    File.WriteAllBytes(_localEncryptionPath, data);
                }
                else
                {
                    var data = File.ReadAllBytes(_localEncryptionPath);
                    var key = ProtectedData.Unprotect(data, _signProvider?.GetPrivateKey(), DataProtectionScope.CurrentUser);
                    exchange = new KeyExchange(key);
                }
                aes.KeySize = 256;
                aes.IV = exchange.IV;
                aes.Key = exchange.Key;
                aes.Padding = PaddingMode.PKCS7;
            }
            else
            {
                aes.KeySize = 256;
                aes.IV = Take(GetPrivateKeyHash(), 16);
                aes.Key = GetPrivateKeyHash();
                aes.Padding = PaddingMode.PKCS7;
            }
            return aes;
        }

        private bool VerifyCommandSign(TransportCommandData commandData)
        {
            if (commandData == null)
                throw new ArgumentNullException(nameof(commandData));

            if (commandData.Certificate?.PublicKey.Key == null || (commandData.Sign == null && commandData.Data?.Length > 0))
            {
                _logger.LogWarning("Incoming signature is invalid");
                return false;
            }
            if (commandData.Sign == null && (commandData.Data?.Length ?? 0) == 0)
            {
                return true;
            }

            if (!ValidateClientCertificate(commandData.Certificate))
            {
                _logger.LogWarning(
                    $"Invalid certificate:\n Server Thumbprint: {RootCert.Thumbprint}\n Client Thumbprint: {commandData.Certificate.Thumbprint}");
                return false;
            }

            bool verifyResult;

            switch (SignAlgorithmType)
            {
                case "RSA":
                case "System.Security.Cryptography.RSA":
                    var rsa = commandData.Certificate.GetRSAPublicKey();
                    verifyResult = rsa.VerifyData(commandData.Data, commandData.Sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    break;
                case "DSA":
                case "System.Security.Cryptography.DSA":
                    var dsa = commandData.Certificate.GetDSAPublicKey();
                    verifyResult = dsa.VerifyData(commandData.Data, commandData.Sign, HashAlgorithmName.SHA256);
                    break;
                case "ECDsa":
                case "ECDsaCng":
                case "System.Security.Cryptography.ECDsaCng":
                    var ecdsa = commandData.Certificate.GetECDsaPublicKey();
                    verifyResult = ecdsa.VerifyData(commandData.Data, commandData.Sign, HashAlgorithmName.SHA256);
                    break;
                default:
                    throw new NotImplementedException($"{SignAlgorithmType} is not implemented");
            }
            if (!verifyResult)
            {
                _logger.LogWarning($"Invalid sign. Remote Certificate Thumbprint: {commandData.Certificate.Thumbprint}, ");
                return false;
            }
            return true;
        }

        private void SignCommand(TransportCommandData commandData)
        {
            if (_signProvider == null)
                return;
            commandData.Sign = _signProvider.SignData(commandData.Data);
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
            var sha = new SHA256Cng();
            sha.Initialize();
            var pk = _signProvider.GetPrivateKey();
            return sha.ComputeHash(pk);
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