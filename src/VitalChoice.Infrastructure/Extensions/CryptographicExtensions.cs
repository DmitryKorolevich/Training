using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Infrastructure.Extensions
{
    public static class CryptographicExtensions
    {
        public static byte[] DecryptBlock(this Aes aes, byte[] data)
        {
            if (aes == null) throw new ArgumentNullException(nameof(aes));

            using (var decryptor = aes.CreateDecryptor())
            {
                return TransformBytes(data, decryptor);
            }
        }

        public static byte[] EncryptBlock(this Aes aes, byte[] data)
        {
            if (aes == null) throw new ArgumentNullException(nameof(aes));

            using (var encryptor = aes.CreateEncryptor())
            {
                return TransformBytes(data, encryptor);
            }
        }

        public static byte[] RewriteBlock(this Aes decryptWith, Aes encryptWith, byte[] data)
        {
            if (decryptWith == null) throw new ArgumentNullException(nameof(decryptWith));
            if (encryptWith == null) throw new ArgumentNullException(nameof(encryptWith));

            using (var decryptor = decryptWith.CreateDecryptor())
            {
                using (var encryptor = encryptWith.CreateEncryptor())
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (CryptoStream de = new CryptoStream(memory, decryptor, CryptoStreamMode.Write))
                        {
                            de.Write(data, 0, data.Length);
                            de.FlushFinalBlock();
                            de.Flush();
                            memory.Seek(0, SeekOrigin.Begin);
                            using (CryptoStream en = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                            {
                                data = memory.ToArray();
                                en.Write(data, 0, data.Length);
                                en.FlushFinalBlock();
                                en.Flush();
                                return memory.ToArray();
                            }
                        }
                    }
                }
            }
        }

        private static byte[] TransformBytes(byte[] data, ICryptoTransform decryptor)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(memory, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return memory.ToArray();
                }
            }
        }
    }
}
