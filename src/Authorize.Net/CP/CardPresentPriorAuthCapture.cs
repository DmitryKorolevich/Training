using System.Globalization;
using Authorize.Net.AIM.Requests;
using Authorize.Net.Utility;

namespace Authorize.Net.CP
{
    /// <summary>
    ///     Captures a prior authorization
    /// </summary>
    public class CardPresentPriorAuthCapture : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentPriorAuthCapture" /> class.
        /// </summary>
        /// <param name="transactionID">The transaction ID.</param>
        /// <param name="amount">The amount.</param>
        public CardPresentPriorAuthCapture(string transactionID, decimal amount)
        {
            SetApiAction(RequestAction.PriorAuthCapture);
            Queue("x_ref_trans_id", transactionID);
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
        }
    }
}