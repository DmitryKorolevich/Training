using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation.Validators;

namespace VitalChoice.Business.Helpers
{
    public static class Extensions
    {
        private static readonly Regex EmailValidationRegex = new Regex(new EmailValidator().Expression,
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
