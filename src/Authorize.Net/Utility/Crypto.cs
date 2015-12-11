using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Authorize.Net.Utility
{
    public class Crypto
    {
        /// <summary>
        ///     Generates the HMAC-encrypted hash to send along with the SIM form
        /// </summary>
        /// <param name="transactionKey">The merchant's transaction key</param>
        /// <param name="login">The merchant's Authorize.NET API Login</param>
        /// <param name="amount">The amount of the transaction</param>
        /// <param name="sequence">The sequence</param>
        /// <param name="timeStamp">The timeStamp</param>
        /// <returns>string</returns>
        public static string GenerateFingerprint(string transactionKey, string login, decimal amount, string sequence, string timeStamp)
        {
            var result = "";
            var keyString = $"{login}^{sequence}^{timeStamp}^{amount}^";
            result = EncryptHMAC(transactionKey, keyString);
            return result;
        }

        /// <summary>
        ///     Decrypts provided string parameter
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="apiLogin">login</param>
        /// <param name="transactionId">transaction id</param>
        /// <param name="amount">amount </param>
        /// <param name="expected">expected string</param>
        /// <returns>string</returns>
        public static bool IsMatch(string key, string apiLogin, string transactionId, decimal amount, string expected)
        {
            var unencrypted = $"{key}{apiLogin}{transactionId}{amount}";

            var md5 = MD5.Create();
            md5.Initialize();
            var hashed = Regex.Replace(BitConverter.ToString(md5.ComputeHash(Encoding.GetEncoding(0).GetBytes(unencrypted))), "-", "");

            // And return it
            return hashed.Equals(expected);
        }

        /// <summary>
        ///     Encrypts the key/value pair supplied using HMAC-MD5
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string EncryptHMAC(string key, string value)
        {
            // The first two lines take the input values and convert them from strings to Byte arrays
            var HMACkey = new ASCIIEncoding().GetBytes(key);
            var HMACdata = new ASCIIEncoding().GetBytes(value);
            var fingerprint = "";
#if DNX451 || NET451
            // create a HMACMD5 object with the key set
            var myhmacMD5 = new HMACMD5(HMACkey);

            //calculate the hash (returns a byte array)
            var HMAChash = myhmacMD5.ComputeHash(HMACdata);
            //loop through the byte array and add append each piece to a string to obtain a hash string
            for (var i = 0; i < HMAChash.Length; i++)
            {
                fingerprint += HMAChash[i].ToString("x").PadLeft(2, '0');
            }
#endif

            return fingerprint;
        }

        /// <summary>
        ///     Generates a 4-place sequence number randomly
        /// </summary>
        /// <returns></returns>
        public static string GenerateSequence()
        {
            var random = new CryptoRandom();
            return random.Next(0, 10000000).ToString();
        }

        /// <summary>
        ///     Generates a timestamp in seconds from 1970
        /// </summary>
        public static int GenerateTimestamp()
        {
            return (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}