using System;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class createCustomerProfileFromTransactionController :
        ApiOperationBase<createCustomerProfileFromTransactionRequest, createCustomerProfileResponse>
    {
        public createCustomerProfileFromTransactionController(createCustomerProfileFromTransactionRequest apiRequest)
            : base(apiRequest)
        {
        }

        protected override void ValidateRequest()
        {
            var request = GetApiRequest();

            //validate required fields		
            if (null == request.transId) throw new ArgumentException("transactionId cannot be null");

            //validate not-required fields		
        }
    }
#pragma warning restore 1591
}