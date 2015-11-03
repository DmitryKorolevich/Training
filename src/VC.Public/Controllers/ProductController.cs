using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Controllers.Content;
using VC.Public.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Controllers
{
    public class ProductController : BaseContentController
    {
        public ProductController(IProductViewService productViewService) : base(productViewService)
        {
        }

	    private IList<CustomerTypeCode> GetCategoryMenuAvailability()
	    {
		    return User.Identity.IsAuthenticated
			    ? (User.IsInRole(IdentityConstants.WholesaleCustomer)
				    ? new List<CustomerTypeCode>() {CustomerTypeCode.Wholesale, CustomerTypeCode.All}
				    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All})
			    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All};
			    //todo: refactor when authentication mechanism gets ready
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await ProductViewService.GetProductCategoryContentAsync(GetCategoryMenuAvailability(), GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await ProductViewService.GetProductCategoryContentAsync(GetCategoryMenuAvailability(), GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}