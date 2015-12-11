using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;

namespace Authorize.Net.Api.Controllers
{
#pragma warning disable 1591
    public class deleteCustomerShippingAddressController :
        ApiOperationBase<deleteCustomerShippingAddressRequest, deleteCustomerShippingAddressResponse>
    {
        public deleteCustomerShippingAddressController(deleteCustomerShippingAddressRequest apiRequest) : base(apiRequest)
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
    }
#pragma warning restore 1591
}