using System;
using System.Security.Cryptography;

namespace Authorize.Net.Utility
{
    public class CryptoRandom
    {
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly byte[] _uint32Buffer = new byte[4];

        public int Next()
        {
            _rng.GetBytes(_uint32Buffer);
            return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue == maxValue) return minValue;
            long diff = maxValue - minValue;
            while (true)
            {
                _rng.GetBytes(_uint32Buffer);
                var rand = BitConverter.ToUInt32(_uint32Buffer, 0);

                var max = 1 + (long) uint.MaxValue;
                var remainder = max%diff;
                if (rand < max - remainder)
                {
                    return (int) (minValue + rand%diff);
                }
            }
        }

        public double NextDouble()
        {
            _rng.GetBytes(_uint32Buffer);
            var rand = BitConverter.ToUInt32(_uint32Buffer, 0);
            return rand/(1.0 + uint.MaxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            _rng.GetBytes(buffer);
        }
    }
}