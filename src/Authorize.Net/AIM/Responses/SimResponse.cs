using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Authorize.Net.Utility;

namespace Authorize.Net.AIM.Responses
{
    public class SimResponse : IGatewayResponse
    {
        private readonly NameValueCollection _post;

        public SimResponse(NameValueCollection post)
        {
            _post = post;
        }

        public string Md5Hash => FindKey("x_MD5_Hash");

        public string CardType => FindKey(ApiFields.CreditCardType);

        public string ResponseCode => FindKey("x_response_code");

        public string ResponseReasonCode => FindKey("x_response_reason_code");

        public string Message => FindKey("x_response_reason_text");

        public bool Approved => ResponseCode == "1";

        public string InvoiceNumber => FindKey(ApiFields.InvoiceNumber);

        public decimal Amount
        {
            get
            {
                var sAmount = FindKey(ApiFields.Amount);
                var result = 0.00M;
                decimal.TryParse(sAmount, out result);
                return result;
            }
        }

        public string TransactionID => FindKey(ApiFields.TransactionID);

        public string AuthorizationCode => FindKey(ApiFields.AuthorizationCode);

        public string CardNumber => FindKey(ApiFields.CreditCardNumber);

        public string GetValueByIndex(int position)
        {
            return ParseResponse(position);
        }

        /// <summary>
        ///     Validates that what was passed by Auth.net is valid
        /// </summary>
        public bool Validate(string merchantHash, string apiLogin)
        {
            return Crypto.IsMatch(merchantHash, apiLogin, TransactionID, Amount, Md5Hash);
        }

        public string GetValue(string name)
        {
            return FindKey(name);
        }

        private string FindKey(string key)
        {
            string result = null;

            if (_post[key] != null)
            {
                result = _post[key];
            }

            return result;
        }

        internal string ParseResponse(int index)
        {
            var result = "";
            if (_post.AllKeys.Count() > index)
            {
                result = _post[index];
            }
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<li>Code = {0}", ResponseCode);
            sb.AppendFormat("<li>Auth = {0}", AuthorizationCode);
            sb.AppendFormat("<li>Message = {0}", Message);
            sb.AppendFormat("<li>TransID = {0}", TransactionID);
            sb.AppendFormat("<li>Approved = {0}", Approved);
            return sb.ToString();
        }
    }
}