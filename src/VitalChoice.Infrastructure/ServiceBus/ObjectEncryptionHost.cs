using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public class ObjectEncryptionHost : IDisposable
    {
        private readonly RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider(4096);
        private readonly Dictionary<Guid, SessionInfo> _sessions = new Dictionary<Guid, SessionInfo>();

        public ObjectEncryptionHost()
        {
            PublicKey = _rsa.ExportParameters(false);
        }

        public ObjectEncryptionHost(RSAParameters keys)
        {
            _rsa.ImportParameters(keys);
            PublicKey = _rsa.ExportParameters(false);
        }

        public RSAParameters PublicKey { get; }

        public T RsaDecrypt<T>(byte[] data)
        {
            var objectData = _rsa.Decrypt(data, true);
            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream(objectData))
            {
                return (T) serializer.Deserialize(memory);
            }
        }

        public byte[] RsaEncrypt<T>(T obj)
        {

            SharpSerializer.Library.SharpSerializer serializer = new SharpSerializer.Library.SharpSerializer(true);
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(obj, memory);
                return _rsa.Encrypt(memory.ToArray(), true);
            }
        }

        public T AesDecrypt<T>(byte[] data, Guid session)
        {
            lock (_sessions)
            {
                SessionInfo encryption;
                if (_sessions.TryGetValue(session, out encryption))
                {
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

        public byte[] AesEncrypt<T>(T obj, Guid session)
        {
            lock (_sessions)
            {
                SessionInfo encryption;
                if (_sessions.TryGetValue(session, out encryption))
                {
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
            Aes aes = Aes.Create();
            if (aes == null)
                throw new InvalidOperationException("Cannot initialize AES encryption");
            aes.Key = key.Key;
            aes.IV = key.IV;
            aes.Mode = CipherMode.CTS;
            aes.Padding = PaddingMode.PKCS7;
            SessionInfo encryption = new SessionInfo
            {
                Aes = aes,
                Encryptor = aes.CreateEncryptor(),
                Decryptor = aes.CreateDecryptor()
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
            lock (_sessions)
            {
                SessionInfo aes;
                if (_sessions.TryGetValue(session, out aes))
                {
                    aes.Encryptor = aes.Aes.CreateEncryptor();
                    aes.Decryptor = aes.Aes.CreateDecryptor();
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
                aes.Mode = CipherMode.CTS;
                aes.Padding = PaddingMode.PKCS7;
                SessionInfo encryption = new SessionInfo
                {
                    Aes = aes,
                    Encryptor = aes.CreateEncryptor(),
                    Decryptor = aes.CreateDecryptor()
                };
                _sessions.Add(session, encryption);
                return new KeyExchange
                {
                    Key = aes.Key,
                    IV = aes.IV
                };
            }
        }

        public void RemoveSession(Guid session)
        {
            lock (_sessions)
            {
                SessionInfo aes;
                if (_sessions.TryGetValue(session, out aes))
                {
                    _sessions.Remove(session);
                    aes.Dispose();
                }
            }
        }

        public void Dispose()
        {
            _rsa.Dispose();
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

            public void Dispose()
            {
                Encryptor?.Dispose();
                Decryptor?.Dispose();
                Aes?.Dispose();
            }
        }
    }
}