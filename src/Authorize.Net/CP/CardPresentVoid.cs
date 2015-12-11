using Authorize.Net.AIM.Requests;

namespace Authorize.Net.CP
{
    /// <summary>
    ///     A Cardpresent Void transaction
    /// </summary>
    public class CardPresentVoid : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentVoid" /> class.
        /// </summary>
        /// <param name="transactionID">The transaction ID.</param>
        public CardPresentVoid(string transactionID)
        {
            SetApiAction(RequestAction.Void);
            Queue("x_ref_trans_id", transactionID);
        }
    }
}