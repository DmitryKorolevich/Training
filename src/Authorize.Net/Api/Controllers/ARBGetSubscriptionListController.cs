using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class ARBGetSubscriptionListController : ApiOperationBase<ARBGetSubscriptionListRequest, ARBGetSubscriptionListResponse>
    {
        public ARBGetSubscriptionListController(ARBGetSubscriptionListRequest apiRequest) : base(apiRequest)
        {
        }

        protected override void ValidateRequest()
        {
            var request = GetApiRequest();
        }
    }
#pragma warning restore 1591
}