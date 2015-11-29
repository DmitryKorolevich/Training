using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class ArrayExtensions
    {
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
