using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VC.Public.Models.Profile;
using VC.Public.Models.Tracking;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Components.Tracking
{
    [ViewComponent(Name = "BodyBeginTrackScripts")]
    public class BodyBeginTrackScriptsComponent : ViewComponent
    {
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly ExtendedUserManager _userManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        public BodyBeginTrackScriptsComponent(IStorefrontUserService storefrontUserService, ExtendedUserManager userManager, IActionContextAccessor actionContextAccessor)
        {
            _storefrontUserService = storefrontUserService;
            _userManager = userManager;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var toReturn = new BodyBeginTrackScriptsModel();

            var userId = Convert.ToInt32(_userManager.GetUserId(_actionContextAccessor.ActionContext.HttpContext.User));

            if (userId != 0)
            {
                var user = await _storefrontUserService.GetAsync(userId);
                if (user != null && user.Status == UserStatus.Active)
                {
                    toReturn.Customer = new CustomerModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };
                }
            }
            return View("~/Views/Shared/Components/Tracking/BodyBeginTrackScripts.cshtml", toReturn);
        }
    }
}