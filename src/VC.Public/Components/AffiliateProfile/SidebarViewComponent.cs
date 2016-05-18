using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models.Profile;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.AffiliateProfile
{
	[ViewComponent(Name = "AffiliateProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IAffiliateUserService _affiliateUserService;

		public SidebarViewComponent(IAffiliateUserService affiliateUserService)
		{
            _affiliateUserService = affiliateUserService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));

			var user = await _affiliateUserService.GetAsync(userId);
			
			return View("Sidebar", new SidebarModel()
			{
                Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName
			});
		}
	}
}