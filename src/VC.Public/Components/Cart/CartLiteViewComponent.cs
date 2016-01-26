using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Menu;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Components.Cart
{
	[ViewComponent(Name = "CartLite")]
	public class CartLiteViewComponent : ViewComponent
	{
		public CartLiteViewComponent()
		{
			
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			return Content("5");
		}
	}
}