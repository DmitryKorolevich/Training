using System.IO;
using System.Security.Cryptography;

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    public class RsaSignProvider : ISignProvider
    {
        private readonly RSA _rsa;

        public RsaSignProvider(RSA rsa)
        {
            _rsa = rsa;
        }

        public byte[] GetPrivateKey()
        {
            using (var memory = new MemoryStream())
            {
                var exportedParams = _rsa.ExportParameters(true);
                memory.Write(exportedParams.D, 0, exportedParams.D.Length);
                memory.Write(exportedParams.DP, 0, exportedParams.DP.Length);
                memory.Write(exportedParams.P, 0, exportedParams.P.Length);
                memory.Write(exportedParams.Q, 0, exportedParams.Q.Length);
                memory.Write(exportedParams.Exponent, 0, exportedParams.Exponent.Length);
                memory.Write(exportedParams.DQ, 0, exportedParams.DQ.Length);
                memory.Write(exportedParams.InverseQ, 0, exportedParams.InverseQ.Length);
                memory.Write(exportedParams.Modulus, 0, exportedParams.Modulus.Length);
                return memory.ToArray();
            }
        }

        public virtual bool VerifyData(byte[] data, byte[] sign)
        {
            return _rsa.VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public virtual byte[] SignData(byte[] data)
        {
            return _rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public void Dispose()
        {
            _rsa.Dispose();
        }
    }
}