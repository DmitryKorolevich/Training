using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Business.Helpers
{
    public static class Extensions
    {
        public static string FormatAsPhone(this string data, string mask)
        {
            if(!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(mask))
            {
                var result = mask.ToCharArray();
                for (int i = 0; i < data.Length; i++)
                {
                    var elem = data[i];
                    if (Char.IsDigit(elem))
                    {
                        int index = -1;
                        for (int j = 0; j < result.Length; j++)
                        {
                            if (result[j] == '_')
                            {
                                index = j;
                                break;
                            }
                        }
                        if (index > 0)
                        {
                            result[index] = elem;
                        }
                    }
                }
                var sResult = new String(result);
                sResult = sResult.Replace("_", "");
                var optionalPartIndex = sResult.IndexOf('?');
                if (optionalPartIndex > -1)
                {
                    var delete = true;
                    for (var i = optionalPartIndex; i < sResult.Length; i++)
                    {
                        if (Char.IsDigit(sResult[i]))
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        sResult = sResult.Substring(0, optionalPartIndex);
                    }
                }
                sResult = sResult.Replace("?", "");
                data = sResult;
            }
            return data;
        }
    }
}
