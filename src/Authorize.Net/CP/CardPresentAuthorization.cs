using System.Globalization;
using Authorize.Net.AIM.Requests;
using Authorize.Net.Utility;

namespace Authorize.Net.CP
{
    public class CardPresentAuthorizeAndCaptureRequest : CardPresentAuthorizationRequest
    {
        public CardPresentAuthorizeAndCaptureRequest(decimal amount, string track1, string track2)
            : base(amount, track1, track2)
        {
            SetApiAction(RequestAction.AuthorizeAndCapture);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentAuthorizeAndCaptureRequest" /> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        public CardPresentAuthorizeAndCaptureRequest(decimal amount, string cardNumber, string expirationMonth, string expirationYear)
            : base(amount, cardNumber, expirationMonth, expirationYear)
        {
            SetApiAction(RequestAction.AuthorizeAndCapture);
        }
    }

    public class CardPresentAuthorizationRequest : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentAuthorizationRequest" /> class using track data from a card
        ///     reader.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="track1">The track1 data.</param>
        /// <param name="track2">The track2 data.</param>
        public CardPresentAuthorizationRequest(decimal amount, string track1, string track2)
        {
            SetApiAction(RequestAction.Authorize);

            //strip the sentinels...
            track1 = track1.Replace("%", "").Replace("?", "");
            track2 = track2.Replace(";", "").Replace("?", "");

            //this.Queue(ApiFields.CreditCardNumber, cardNumber);
            if (!string.IsNullOrEmpty(track1))
            {
                Queue("x_track1", track1);
            }

            if (!string.IsNullOrEmpty(track2))
            {
                Queue("x_track2", track2);
            }
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentAuthorizationRequest" /> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        public CardPresentAuthorizationRequest(decimal amount, string cardNumber, string expirationMonth, string expirationYear)
        {
            SetApiAction(RequestAction.Authorize);
            Queue(ApiFields.CreditCardNumber, cardNumber);
            Queue(ApiFields.CreditCardExpiration, $"{expirationMonth}{expirationYear}");
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}