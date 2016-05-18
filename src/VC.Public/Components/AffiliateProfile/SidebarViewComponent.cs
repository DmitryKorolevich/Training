using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models.Profile;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.AffiliateProfile
{
	[ViewComponent(Name = "AffiliateProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IAffiliateUserService _affiliateUserService;
	    private readonly ExtendedUserManager _userManager;

	    public SidebarViewComponent(IAffiliateUserService affiliateUserService, ExtendedUserManager userManager)
	    {
	        _affiliateUserService = affiliateUserService;
	        _userManager = userManager;
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