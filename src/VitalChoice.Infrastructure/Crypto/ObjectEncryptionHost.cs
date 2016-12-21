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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.Infrastructure.Crypto
{
    public delegate void SessionExpiredEventHandler(Guid session, string hostName);

    public class ObjectEncryptionHost : IObjectEncryptionHost
    {
        protected static string SignAlgorithmType => "ECDsaCng";

        private readonly string _localEncryptionPath;
        private readonly ILogger _logger;

        private readonly ISignProvider _signProvider;
        private readonly AesCng _localAes;
        private readonly Dictionary<Guid, SessionInfo> _sessions = new Dictionary<Guid, SessionInfo>();
        private readonly ManualResetEvent _disposeEvent;
        private readonly Thread _sessionExpiration;
        private readonly IReadOnlyDictionary<string, ISignCheckProvider> _remoteSignCheckProviders;

        /// <summary>
        /// Server Mode
        /// </summary>
        public ObjectEncryptionHost(IOptions<AppOptions> options, ILoggerFactory logger)
        {
            _localEncryptionPath = options.Value.LocalEncryptionKeyPath;
            _logger = logger.CreateLogger<ObjectEncryptionHost>();
            try
            {
                X509Certificate2 cert;
                _signProvider = GetSignProvider(StoreName.My, options.Value.ExportService.CertThumbprint, out cert);
                if (cert == null)
                {
                    _signProvider?.Dispose();
                    _signProvider = null;
                }
                else
                {
                    LocalCert = cert;
                }

                RootCert = GetCaCertificate(options.Value.ExportService.RootThumbprint);
                if (RootCert == null)
                {
                    _localAes = GetLocalAes(true);
                    return;
                }
                _remoteSignCheckProviders = GetPublicSignCheckProviders(StoreName.My, RootCert);
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
                _logger.LogCritical(e.ToString());
                _localAes = GetLocalAes(true);
            }
        }

        public X509Certificate2 RootCert { get; }

        public byte[] HashBytes(byte[] data)
        {
            var sha = new SHA256Cng();
            sha.Initialize();
            return sha.ComputeHash(data, 0, data.Length);
        }

        public string HashString(string str)
        {
            var sha = new SHA256Cng();
            sha.Initialize();
            var data = Encoding.Unicode.GetBytes(str);
            return sha.ComputeHash(data, 0, data.Length).ToHexString();
        }

        public void UpdateLocalKey(KeyExchange key)
        {
            var data = ProtectedData.Protect(key.ToCombined(), Encoding.Unicode.GetBytes(SignAlgorithmType),
                DataProtectionScope.LocalMachine);
            File.WriteAllBytes(_localEncryptionPath, data);
            _localAes.Key = key.Key;
            _localAes.IV = key.IV;
        }

        public KeyExchange GetLocalKey()
        {
            return new KeyExchange(_localAes.Key, _localAes.IV);
        }

        public X509Certificate2 LocalCert { get; }

        public T LocalDecrypt<T>(byte[] data) => (T) ObjectSerializer.Deserialize(Decrypt(_localAes, data));

        public byte[] LocalEncrypt(object obj) => Encrypt(_localAes, ObjectSerializer.Serialize(obj));

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

        public T AesDecryptVerify<T>(TransportCommandData command, Guid session)
            where T : ServiceBusCommandBase
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (command.Data == null)
            {
                _logger.LogWarning("Empty command data");
                return default(T);
            }
            SessionInfo encryption;
            lock (_sessions)
            {
                if (!_sessions.TryGetValue(session, out encryption))
                {
                    _logger.LogWarning($"Session does not exist: {session}");
                    return default(T);
                }
            }
            if (VerifyCommandSign(command))
            {
                byte[] plainData = null;
                try
                {
                    plainData = Decrypt(encryption.Aes, command.Data);
                    return (T) ObjectSerializer.Deserialize(plainData);
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
                var plainData = ObjectSerializer.Serialize(command);
                var encryptedData = Encrypt(encryption.Aes, plainData);
                var result = new TransportCommandData(encryptedData);
                SignCommand(result);
                return result;
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

        public bool RegisterSession(Guid session, string hostName, KeyExchange keyExchange)
        {
            AesCng aes = new AesCng();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.KeySize = 256;
            aes.Key = keyExchange.Key;
            aes.IV = keyExchange.IV;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo(aes, hostName);
            lock (_sessions)
            {
                if (_sessions.ContainsKey(session))
                    return false;
                _sessions.Add(session, encryption);
            }
            return true;
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
            _signProvider?.Dispose();
            lock (_sessions)
            {
                foreach (var session in _sessions)
                {
                    session.Value.Dispose();
                }
                _sessions.Clear();
            }
            if (_remoteSignCheckProviders != null)
            {
                foreach (var signCheckProvider in _remoteSignCheckProviders.Values)
                {
                    signCheckProvider.Dispose();
                }
            }
            _disposeEvent?.Dispose();
        }

        public Guid GetSession()
        {
            lock (_sessions)
            {
                var newSession = Guid.NewGuid();
                var keys = CreateSession(newSession);
                RegisterSession(newSession, keys);
                return newSession;
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

        private void RegisterSession(Guid session, KeyExchange keyCombined)
        {
            var aes = new AesCng
            {
                KeySize = 256,
                Key = keyCombined.Key,
                IV = keyCombined.IV,
                Padding = PaddingMode.PKCS7
            };
            SessionInfo encryption = new SessionInfo(aes);
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
                var aes = new AesCng {KeySize = 256};
                aes.GenerateKey();
                aes.GenerateIV();
                aes.Padding = PaddingMode.PKCS7;
                return new KeyExchange(aes.Key, aes.IV);
            }
        }

        private X509Certificate2 GetCaCertificate(string thumbprint)
        {
            var certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
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
                return new X509Certificate2(cert.Export(X509ContentType.Cert));
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

        private ISignProvider GetSignProvider(StoreName storeName, string thumbprint, out X509Certificate2 result)
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
                return CreateSignProvider(cert);
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

        private Dictionary<string, ISignCheckProvider> GetPublicSignCheckProviders(StoreName storeName, X509Certificate2 rootCa)
        {
            var certStore = new X509Store(storeName, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);
            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var localCerts = certStore.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, rootCa.IssuerName.Name, true);
                Dictionary<string, ISignCheckProvider> result = new Dictionary<string, ISignCheckProvider>();
                foreach (var localCert in localCerts)
                {
                    if (rootCa.Thumbprint != localCert.Thumbprint && !localCert.HasPrivateKey)
                    {
                        if (ValidateClientCertificate(localCert, rootCa))
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute
                            result.Add(localCert.Thumbprint, CreateSignCheckProvider(localCert));
                        }
                        else
                        {
                            _logger.LogWarning($"{localCert.Thumbprint} is invalid in whole chain with root: {rootCa.Thumbprint}");
                        }
                    }
                }
                return result;
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

        private ISignCheckProvider CreateSignCheckProvider(X509Certificate2 cert)
        {
            switch (SignAlgorithmType)
            {
                case "RSA":
                case "System.Security.Cryptography.RSA":
                    return new RsaSignProvider(cert.GetRSAPublicKey());
                case "DSA":
                case "System.Security.Cryptography.DSA":
                    return new DsaSignProvider(cert.GetDSAPublicKey());
                case "ECDsa":
                case "ECDsaCng":
                case "System.Security.Cryptography.ECDsaCng":
                    return new ECDsaSignProvider(cert.GetECDsaPublicKey());
                default:
                    throw new NotImplementedException($"{SignAlgorithmType} is not supported");
            }
        }

        private ISignProvider CreateSignProvider(X509Certificate2 cert)
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
                lock (_sessions)
                {
                    var expiredSessions =
                        _sessions.Where(session => session.Value.Expiration <= DateTime.Now).ToArray();
                    foreach (var expiredSession in expiredSessions)
                    {
                        OnSessionExpired?.Invoke(expiredSession.Key, expiredSession.Value.HostName);
                        RemoveSession(expiredSession.Key);
                    }
                }
            }
        }

        private AesCng GetLocalAes(bool randomize = false)
        {
            var aes = new AesCng
            {
                KeySize = 256,
                Padding = PaddingMode.PKCS7
            };
            if (!randomize)
            {
                if (!string.IsNullOrWhiteSpace(_localEncryptionPath))
                {
                    KeyExchange exchange;
                    if (File.Exists(_localEncryptionPath))
                    {
                        var data = File.ReadAllBytes(_localEncryptionPath);
                        var key = ProtectedData.Unprotect(data, Encoding.Unicode.GetBytes(SignAlgorithmType),
                            DataProtectionScope.LocalMachine);
                        exchange = new KeyExchange(key);
                        aes.IV = exchange.IV;
                        aes.Key = exchange.Key;
                    }
                    else
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
                        aes.GenerateKey();
                        aes.GenerateKey();
                        exchange = new KeyExchange(aes.Key, aes.IV);
                        var data = ProtectedData.Protect(exchange.ToCombined(), Encoding.Unicode.GetBytes(SignAlgorithmType),
                            DataProtectionScope.LocalMachine);
                        File.WriteAllBytes(_localEncryptionPath, data);
                    }
                }
                else
                {
                    _logger.LogWarning("No key path specified, using random local key");
                    aes.GenerateKey();
                    aes.GenerateKey();
                }
            }
            else
            {
                _logger.LogWarning("Initialization failed, using random local key");
                aes.GenerateKey();
                aes.GenerateKey();
            }
            return aes;
        }

        private bool VerifyCommandSign(TransportCommandData commandData)
        {
            if (commandData == null)
                throw new ArgumentNullException(nameof(commandData));

            ISignCheckProvider signProvider;
            if (!_remoteSignCheckProviders.TryGetValue(commandData.CertThumbprint, out signProvider))
            {
                _logger.LogWarning("Incoming certificate thumbprint is invalid");
                return false;
            }
            if (commandData.Sign == null && commandData.Data?.Length > 0)
            {
                _logger.LogWarning("Incoming signature is invalid");
                return false;
            }
            if (commandData.Sign == null && (commandData.Data?.Length ?? 0) == 0)
            {
                _logger.LogWarning("Empty command without data and sign, processing as normal.");
                return true;
            }

            if (!signProvider.VerifyData(commandData.Data, commandData.Sign))
            {
                _logger.LogWarning($"Invalid sign. Remote Certificate Thumbprint: {commandData.CertThumbprint}, ");
                return false;
            }
            return true;
        }

        private void SignCommand(TransportCommandData commandData)
        {
            if (_signProvider == null)
                return;
            commandData.Sign = _signProvider.SignData(commandData.Data);
            commandData.CertThumbprint = LocalCert.Thumbprint;
        }

        private static bool ValidateClientCertificate(X509Certificate2 clientCert, X509Certificate2 rootCa)
        {
            if (clientCert == null)
                throw new ArgumentNullException(nameof(clientCert));
            if (clientCert == null)
                throw new ArgumentNullException(nameof(rootCa));

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
            validationChain.ChainPolicy.ExtraStore.Add(rootCa);
            return validationChain.Build(clientCert);
        }

        private byte[] Encrypt(AesCng aes, byte[] data)
        {
            if (aes == null || data == null)
            {
                return new byte[0];
            }

            using (var encryptor = aes.CreateEncryptor())
            {
                return TransformBlocks(data, encryptor);
            }
        }

        private byte[] Decrypt(AesCng aes, byte[] encryptedData)
        {
            if (aes == null || encryptedData == null)
            {
                return new byte[0];
            }

            using (var decryptor = aes.CreateDecryptor())
            {
                return TransformBlocks(encryptedData, decryptor);
            }
        }

        private static byte[] TransformBlocks(byte[] data, ICryptoTransform transform)
        {
            return transform.TransformFinalBlock(data, 0, data.Length);
        }

        private class SessionInfo : IDisposable
        {
            public SessionInfo(AesCng aes, string hostName = null)
            {
                if (aes == null)
                    throw new ArgumentNullException(nameof(aes));

                Aes = aes;
                HostName = hostName;
            }

            public AesCng Aes { get; }
            public DateTime Expiration { get; } = DateTime.Now.AddHours(1);
            public string HostName { get; }

            public void Dispose() => Aes.Dispose();
        }
    }
}