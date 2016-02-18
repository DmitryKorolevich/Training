using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Menu;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Components.Menu
{
	[ViewComponent(Name = "Menu")]
	public class MenuViewComponent : ViewComponent
	{
		private readonly IProductCategoryService _productCategoryService;

		public MenuViewComponent(IProductCategoryService productCategoryService)
		{
			_productCategoryService = productCategoryService;
		}

		private IList<MainMenuIItemModel> ConvertToModel(IList<ProductNavCategoryLite> entities)
		{
			return entities.Select(x => new MainMenuIItemModel()
			{
				Label = !string.IsNullOrWhiteSpace(x.NavLabel) ? x.NavLabel : x.ProductCategory.Name,
				Link = !string.IsNullOrWhiteSpace(x.Url) ? $"/products/{x.Url}" : string.Empty,
				SubItems = ConvertToModel(x.SubItems)
			}).ToList();
		}

	    public async Task<IViewComponentResult> InvokeAsync()
	    {
	        var rootCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(new ProductCategoryLiteFilter()
	        {
	            Visibility =
	                User.Identity.IsAuthenticated
	                    ? (User.IsInRole(IdentityConstants.WholesaleCustomer)
	                        ? new List<CustomerTypeCode>() {CustomerTypeCode.Wholesale, CustomerTypeCode.All}
	                        : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All})
	                    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All},
	            Statuses = new List<RecordStatusCode>() {RecordStatusCode.Active}
	        });

	        return View("MainMenu", new MainMenuIItemModel()
	        {
	            SubItems = ConvertToModel(rootCategory.SubItems)
	        });
	    }
	}
}