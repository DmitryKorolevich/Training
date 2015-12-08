using System.Globalization;
using Authorize.Net.Utility;

namespace Authorize.Net.AIM.Requests
{
    /// <summary>
    ///     A request representing a Capture - the final transfer of funds that happens after an auth.
    /// </summary>
    public class CaptureRequest : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CaptureRequest" /> class.
        /// </summary>
        /// <param name="authCode">The auth code.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonthAndYear">The expiration month and year.</param>
        /// <param name="amount">The amount.</param>
        public CaptureRequest(string authCode, string cardNumber, string expirationMonthAndYear, decimal amount)
        {
            SetApiAction(RequestAction.Capture);
            Queue(ApiFields.AuthorizationCode, authCode);
            Queue(ApiFields.CreditCardNumber, cardNumber);
            Queue(ApiFields.CreditCardExpiration, expirationMonthAndYear);
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}