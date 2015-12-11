using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class logoutController : ApiOperationBase<logoutRequest, logoutResponse>
    {
        public logoutController(logoutRequest apiRequest) : base(apiRequest)
        {
        }

        protected override void ValidateRequest()
        {
            var request = GetApiRequest();

            //validate required fields		
            //if ( 0 == request.SearchType) throw new ArgumentException( "SearchType cannot be null");
            //if ( null == request.Paging) throw new ArgumentException("Paging cannot be null");

            //validate not-required fields		
        }

        protected override void BeforeExecute()
        {
            var request = GetApiRequest();
            RequestFactoryWithSpecified.logoutRequest(request);
        }
    }
#pragma warning restore 1591
}