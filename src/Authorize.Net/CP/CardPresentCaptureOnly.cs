using System.Globalization;
using Authorize.Net.AIM.Requests;
using Authorize.Net.Utility;

namespace Authorize.Net.CP
{
    /// <summary>
    ///     Capture only function
    /// </summary>
    public class CardPresentCaptureOnly : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentCaptureOnly" /> class.
        /// </summary>
        /// <param name="authCode">The auth code.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonthAndYear">The expiration month and year.</param>
        /// <param name="amount">The amount.</param>
        public CardPresentCaptureOnly(string authCode, string cardNumber, string expirationMonthAndYear, decimal amount)
        {
            SetApiAction(RequestAction.Capture);
            Queue(ApiFields.AuthorizationCode, authCode);
            Queue(ApiFields.CreditCardNumber, cardNumber);
            Queue(ApiFields.CreditCardExpiration, expirationMonthAndYear);
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}