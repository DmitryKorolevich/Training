using Authorize.Net.Utility;

namespace Authorize.Net.AIM.Requests
{
    /// <summary>
    ///     A request representing a Void of a previously authorized transaction
    /// </summary>
    public class VoidRequest : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoidRequest" /> class.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        public VoidRequest(string transactionId)
        {
            SetApiAction(RequestAction.Void);
            Queue(ApiFields.TransactionID, transactionId);
        }
    }
}