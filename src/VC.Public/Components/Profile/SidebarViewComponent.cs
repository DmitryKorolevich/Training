using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VC.Public.Models.Profile;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.Profile
{
	[ViewComponent(Name = "ProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IStorefrontUserService _storefrontUserService;
	    private readonly ExtendedUserManager _userManager;
	    private readonly IActionContextAccessor _actionContextAccessor;

	    public SidebarViewComponent(IStorefrontUserService storefrontUserService, ExtendedUserManager userManager, IActionContextAccessor actionContextAccessor)
	    {
	        _storefrontUserService = storefrontUserService;
	        _userManager = userManager;
	        _actionContextAccessor = actionContextAccessor;
	    }

	    public async Task<IViewComponentResult> InvokeAsync()
		{
			var userId = Convert.ToInt32(_userManager.GetUserId(_actionContextAccessor.ActionContext.HttpContext.User));

			var user = await _storefrontUserService.GetAsync(userId);
		    if (user != null && user.Status == UserStatus.Active)
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