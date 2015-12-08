using System;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class getCustomerPaymentProfileListController :
        ApiOperationBase<getCustomerPaymentProfileListRequest, getCustomerPaymentProfileListResponse>
    {
        public getCustomerPaymentProfileListController(getCustomerPaymentProfileListRequest apiRequest) : base(apiRequest)
        {
        }

        protected override void ValidateRequest()
        {
            var request = GetApiRequest();

            //validate required fields		
            if (request.searchType < 0) throw new ArgumentException("SearchType cannot be null");
            if (request.month == null) throw new ArgumentException("month cannot be null");

            //if ( null == request.Paging) throw new ArgumentException("Paging cannot be null");

            //validate not-required fields		
        }

        protected override void BeforeExecute()
        {
            var request = GetApiRequest();
            RequestFactoryWithSpecified.getCustomerPaymentProfileListRequest(request);
        }
    }
#pragma warning restore 1591
}