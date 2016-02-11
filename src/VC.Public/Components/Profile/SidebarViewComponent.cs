using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Profile;
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
		    if (user != null)
		    {

		        return View("Sidebar", new SidebarModel()
		        {
		            FirstName = user.FirstName,
		            LastName = user.LastName,
		            Id = user.Id
		        });
		    }
		    return View("Sidebar", new SidebarModel());
		}
	}
}