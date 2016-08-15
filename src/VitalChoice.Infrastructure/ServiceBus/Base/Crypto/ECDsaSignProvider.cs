using System.Security.Cryptography;
using System.Text;

namespace VitalChoice.Infrastructure.ServiceBus.Base.Crypto
{
    // ReSharper disable once InconsistentNaming
    public class ECDsaSignProvider : ISignProvider
    {
        private readonly ECDsa _ecdsa;

        public ECDsaSignProvider(ECDsa ecdsa)
        {
            _ecdsa = ecdsa;
        }

        public byte[] GetPrivateKey()
        {
            return Encoding.ASCII.GetBytes((_ecdsa as ECDsaCng)?.ToXmlString(ECKeyXmlFormat.Rfc4050) ?? string.Empty);
        }

        public virtual bool VerifyData(byte[] data, byte[] sign)
        {
            return _ecdsa.VerifyData(data, sign, HashAlgorithmName.SHA256);
        }

        public virtual byte[] SignData(byte[] data)
        {
            return _ecdsa.SignData(data, HashAlgorithmName.SHA256);
        }

        public void Dispose()
        {
            _ecdsa.Dispose();
        }
    }
}