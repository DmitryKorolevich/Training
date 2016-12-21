using System.IO;
using System.Security.Cryptography;

namespace VitalChoice.Infrastructure.Crypto
{
    public class DsaSignProvider : ISignProvider
    {
        private readonly DSA _dsa;

        public DsaSignProvider(DSA dsa)
        {
            _dsa = dsa;
        }

        public byte[] GetPrivateKey()
        {
            using (var memory = new MemoryStream())
            {
                var exportedParams = _dsa.ExportParameters(true);
                memory.Write(exportedParams.G, 0, exportedParams.G.Length);
                memory.Write(exportedParams.J, 0, exportedParams.J.Length);
                memory.Write(exportedParams.P, 0, exportedParams.P.Length);
                memory.Write(exportedParams.Q, 0, exportedParams.Q.Length);
                memory.Write(exportedParams.Seed, 0, exportedParams.Seed.Length);
                memory.Write(exportedParams.X, 0, exportedParams.X.Length);
                memory.Write(exportedParams.Y, 0, exportedParams.Y.Length);
                return memory.ToArray();
            }
        }

        public virtual bool VerifyData(byte[] data, byte[] sign)
        {
            return _dsa.VerifyData(data, sign, HashAlgorithmName.SHA256);
        }

        public virtual byte[] SignData(byte[] data)
        {
            return _dsa.SignData(data, HashAlgorithmName.SHA256);
        }

        public void Dispose()
        {
            _dsa.Dispose();
        }
    }
}