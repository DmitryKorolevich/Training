using System;
using System.Globalization;

namespace Authorize.Net.Utility
{
    /// <summary>
    ///     NullableBooleanEnum
    /// </summary>
    public enum NullableBooleanEnum
    {
        False = 0,
        True = 1,
        Null = 2
    }

    public class CommonFunctions
    {
        public static bool ParseDateTime(int year, int month, int day, out DateTime dt)
        {
            var bRet = false;
            dt = new DateTime();
            try
            {
                var culture = new CultureInfo("en-US");
                bRet = DateTime.TryParse(month + "-1-" + year, culture, DateTimeStyles.None, out dt);
            }
            catch (Exception)
            {
                bRet = false;
            }
            return bRet;
        }
    }
}