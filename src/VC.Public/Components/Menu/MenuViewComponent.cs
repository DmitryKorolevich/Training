using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Menu;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
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

		private IList<MainMenuIItemModel> ConvertToModel(IList<ProductCategoryLite> entities)
		{
			return entities.Select(x => new MainMenuIItemModel()
			{
				Label = x.Label,
				Link = x.Link,
				SubItems = ConvertToModel(x.SubItems)
			}).ToList();
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var rootCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(new ProductCategoryLiteFilter()
			{
				Visibility = User.Identity.IsAuthenticated ? (User.IsInRole("Retail") ? CustomerTypeCode.Retail : CustomerTypeCode.Wholesale ) : CustomerTypeCode.All //refactor when authentication mechanism gets ready
			});
			
			return View("MainMenu", new MainMenuIItemModel()
			{
				Label = rootCategory.Label,
				Link = rootCategory.Link,
				SubItems = ConvertToModel(rootCategory.SubItems)
			});
		}
	}
}