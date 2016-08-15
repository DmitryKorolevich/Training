using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain
{
    public static class Extensions
    {
        private static readonly Regex EmailValidationRegex = new Regex(
            @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(this string text)
        {
            return !string.IsNullOrWhiteSpace(text) && EmailValidationRegex.IsMatch(text);
        }

        public static string ClearPhone(this string data)
        {
            return !string.IsNullOrEmpty(data)
                ? data.Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Replace("x", "")
                    .Replace("_", "")
                : data;
        }

        public static string FormatAsPhone(this string data, string mask)
        {
            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(mask))
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

        public static DateTime? GetDateFromQueryStringInPst(this string value, TimeZoneInfo pst)
        {
            DateTime? toReturn = null;
            DateTimeOffset temp;
            if (!DateTimeOffset.TryParse(value, out temp))
            {
                return null;
            }
            toReturn = temp.DateTime;
            toReturn = TimeZoneInfo.ConvertTime(toReturn.Value, pst, TimeZoneInfo.Local);

            return toReturn;
        }
    }
}
