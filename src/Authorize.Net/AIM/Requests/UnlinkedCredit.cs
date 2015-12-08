using System.Globalization;
using Authorize.Net.Utility;

namespace Authorize.Net.AIM.Requests
{
    public class UnlinkedCredit : GatewayRequest
    {
        public UnlinkedCredit(decimal amount, string cardNumber, string expirationMonthAndYear)
        {
            SetApiAction(RequestAction.UnlinkedCredit);
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
            Queue(ApiFields.CreditCardNumber, cardNumber);
            Queue(ApiFields.CreditCardExpiration, expirationMonthAndYear);
        }
    }
}