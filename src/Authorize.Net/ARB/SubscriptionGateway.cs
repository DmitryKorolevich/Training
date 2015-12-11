using System.Threading.Tasks;
using Authorize.Net.AIM;
using Authorize.Net.Utility;

namespace Authorize.Net.ARB
{
    public class SubscriptionGateway : ISubscriptionGateway
    {
        private readonly HttpXmlUtility _gateway;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubscriptionGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        /// <param name="mode">The mode.</param>
        public SubscriptionGateway(string apiLogin, string transactionKey, ServiceMode mode)
        {
            if (mode == ServiceMode.Live)
            {
                _gateway = new HttpXmlUtility(ServiceMode.Live, apiLogin, transactionKey);
            }
            else
            {
                _gateway = new HttpXmlUtility(ServiceMode.Test, apiLogin, transactionKey);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubscriptionGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        public SubscriptionGateway(string apiLogin, string transactionKey) : this(apiLogin, transactionKey, ServiceMode.Test)
        {
        }

        /// <summary>
        ///     Creates a new subscription
        /// </summary>
        /// <param name="subscription">The subscription to create - requires that you add a credit card and billing first and last.</param>
        public async Task<ISubscriptionRequest> CreateSubscription(ISubscriptionRequest subscription)
        {
            var sub = subscription.ToAPI();
            var req = new ARBCreateSubscriptionRequest {subscription = sub};
            var response = (ARBCreateSubscriptionResponse) await _gateway.Send(req);
            subscription.SubscriptionID = response.subscriptionId;
            return subscription;
        }

        /// <summary>
        ///     Updates the subscription.
        /// </summary>
        /// <param name="subscription">The subscription to update. Can't change billing intervals however.</param>
        /// <returns></returns>
        public async Task<bool> UpdateSubscription(ISubscriptionRequest subscription)
        {
            var sub = subscription.ToUpdateableAPI();
            var req = new ARBUpdateSubscriptionRequest
            {
                subscription = sub,
                subscriptionId = subscription.SubscriptionID
            };
            var response = (ARBUpdateSubscriptionResponse) await _gateway.Send(req);
            return true;
        }

        /// <summary>
        ///     Cancels the subscription.
        /// </summary>
        /// <param name="subscriptionID">The subscription ID.</param>
        /// <returns></returns>
        public async Task<bool> CancelSubscription(string subscriptionID)
        {
            var req = new ARBCancelSubscriptionRequest {subscriptionId = subscriptionID};

            //will throw if there are errors
            var response = (ARBCancelSubscriptionResponse) await _gateway.Send(req);
            return true;
        }

        /// <summary>
        ///     Gets the subscription status.
        /// </summary>
        /// <param name="subscriptionID">The subscription ID.</param>
        /// <returns></returns>
        public async Task<ARBSubscriptionStatusEnum> GetSubscriptionStatus(string subscriptionID)
        {
            var req = new ARBGetSubscriptionStatusRequest {subscriptionId = subscriptionID};
            var response = (ARBGetSubscriptionStatusResponse) await _gateway.Send(req);
            return response.status;
        }
    }
}