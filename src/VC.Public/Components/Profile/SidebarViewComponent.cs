using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Profile;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.Profile
{
	[ViewComponent(Name = "ProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IStorefrontUserService _storefrontUserService;

		public SidebarViewComponent(IStorefrontUserService storefrontUserService)
		{
			_storefrontUserService = storefrontUserService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var userId = Convert.ToInt32(HttpContext.User.GetUserId());

			var user = await _storefrontUserService.GetAsync(userId);
		    if (user != null && user.IsConfirmed && user.Status == UserStatus.Active)
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