using Authorize.Net.AIM.Requests;

namespace Authorize.Net.CP
{
    /// <summary>
    ///     A Credit transaction
    /// </summary>
    public class CardPresentCredit : GatewayRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CardPresentCredit" /> class.
        /// </summary>
        /// <param name="transactionID">The transaction ID.</param>
        public CardPresentCredit(string transactionID)
        {
            SetApiAction(RequestAction.Credit);
            Queue("x_ref_trans_id", transactionID);
        }
    }
}