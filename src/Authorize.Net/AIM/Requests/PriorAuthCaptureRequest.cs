using System.Globalization;
using Authorize.Net.Utility;

namespace Authorize.Net.AIM.Requests
{
    /// <summary>
    ///     A request representing a PriorAuthCapture - the final transfer of funds that happens after an auth.
    /// </summary>
    public class PriorAuthCaptureRequest : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PriorAuthCaptureRequest" /> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="transactionId">The transaction id.</param>
        public PriorAuthCaptureRequest(decimal amount, string transactionId)
        {
            SetApiAction(RequestAction.PriorAuthCapture);
            Queue(ApiFields.Amount, amount.ToString(CultureInfo.InvariantCulture));
            Queue(ApiFields.TransactionID, transactionId);
        }
    }
}