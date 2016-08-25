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
        public static byte[] RewriteBlock(this AesCng decryptWith, Aes encryptWith, byte[] data)
        {
            if (decryptWith == null) throw new ArgumentNullException(nameof(decryptWith));
            if (encryptWith == null) throw new ArgumentNullException(nameof(encryptWith));

            using (var decryptor = decryptWith.CreateDecryptor())
            {
                using (var encryptor = encryptWith.CreateEncryptor())
                {
                    var decrypted = decryptor.TransformFinalBlock(data, 0, data.Length);
                    return encryptor.TransformFinalBlock(decrypted, 0, decrypted.Length);
                }
            }
        }
    }
}