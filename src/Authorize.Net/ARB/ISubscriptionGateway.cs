using System.Threading.Tasks;
using Authorize.Net.Utility;

namespace Authorize.Net.ARB
{
    public interface ISubscriptionGateway
    {
        Task<bool> CancelSubscription(string subscriptionID);
        Task<ISubscriptionRequest> CreateSubscription(ISubscriptionRequest subscription);
        Task<ARBSubscriptionStatusEnum> GetSubscriptionStatus(string subscriptionID);
        Task<bool> UpdateSubscription(ISubscriptionRequest subscription);
    }
}