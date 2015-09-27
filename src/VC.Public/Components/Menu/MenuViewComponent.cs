using System;
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

		private IList<MainMenuIItemModel> ConvertToModel(IList<ProductNavCategoryLite> entities)
		{
			return entities.Select(x => new MainMenuIItemModel()
			{
				Label = x.Label,
				Link = !string.IsNullOrWhiteSpace(x.Link) ? $"/products/{x.Link}" : string.Empty,
				SubItems = ConvertToModel(x.SubItems)
			}).ToList();
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var rootCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(new ProductCategoryLiteFilter()
			{
				Visibility = User.Identity.IsAuthenticated ? (User.IsInRole("Wholesale") ? new List<CustomerTypeCode>() { CustomerTypeCode.Wholesale, CustomerTypeCode.All } : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All })
				: new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All }, //todo: refactor when authentication mechanism gets ready
				Statuses = new List<RecordStatusCode>() { RecordStatusCode.Active }
			});
			
			return View("MainMenu", new MainMenuIItemModel()
			{
				SubItems = ConvertToModel(rootCategory.SubItems)
			});
		}
	}
}