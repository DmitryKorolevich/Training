using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Profile;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.AffiliateProfile
{
	[ViewComponent(Name = "AffiliateProfileSidebar")]
	public class SidebarViewComponent : ViewComponent
	{
		private readonly IAffiliateUserService _affiliateUserService;
		private readonly IHttpContextAccessor _contextAccessor;

		public SidebarViewComponent(IAffiliateUserService affiliateUserService, IHttpContextAccessor contextAccessor)
		{
            _affiliateUserService = affiliateUserService;
			_contextAccessor = contextAccessor;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var userId = Convert.ToInt32(_contextAccessor.HttpContext.User.GetUserId());

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