using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Menu;
using VC.Public.Models.Profile;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.Profile
{
	[ViewComponent(Name = "ProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IStorefrontUserService _storefrontUserService;
		private readonly IHttpContextAccessor _contextAccessor;

		public SidebarViewComponent(IStorefrontUserService storefrontUserService, IHttpContextAccessor contextAccessor)
		{
			_storefrontUserService = storefrontUserService;
			_contextAccessor = contextAccessor;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var userId = Convert.ToInt32(_contextAccessor.HttpContext.User.GetUserId());

			var user = await _storefrontUserService.GetAsync(userId);
			
			return View("Sidebar", new SidebarModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Id = user.Id
			});
		}
	}
}