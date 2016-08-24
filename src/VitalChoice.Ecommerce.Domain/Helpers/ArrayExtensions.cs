using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class ArrayExtensions
    {
        public static string ToHexString(this byte[] data)
        {
            var builder = new StringBuilder(data.Length * 2);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("X2"));
            }
            return builder.ToString();
        }

        public static byte[] FromHexString(this string hexData)
        {
            if (string.IsNullOrEmpty(hexData))
                return null;
            try
            {
                var result = new byte[hexData.Length/2];
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = byte.Parse(hexData.Substring(i*2, 2), NumberStyles.HexNumber);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static unsafe bool AreEqualsTo(this byte[] src, byte[] dest)
        {
            fixed (byte* sp = src)
            {
                fixed (byte* dp = dest)
                {
                    return Equals(sp, dp, src.Length, dest.Length) == 0;
                }
            }
        }

        internal static unsafe int Equals(byte* one, byte* two, int lenOne, int lenTwo)
        {
            if (one == two)
            {
                return 0;
            }
            int length = lenOne <= lenTwo ? lenOne : lenTwo;
            if (lenOne != lenTwo)
                return lenOne - lenTwo;
            while (length > 0 && *one == *two)
            {
                one++;
                two++;
                length--;
            }

            if (length > 0)
            {
                return *one - *two;
            }
            return 0;
        }

    }
}