using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class isAliveController : ApiOperationBase<ANetApiRequest, isAliveResponse>
    {
        public isAliveController(ANetApiRequest apiRequest)
            : base(apiRequest)
        {
        }

        protected override void ValidateRequest()
        {
            var request = GetApiRequest();
        }

        protected override void BeforeExecute()
        {
            var request = GetApiRequest();
            RequestFactoryWithSpecified.isAliveRequest(request);
        }
    }
#pragma warning restore 1591
}