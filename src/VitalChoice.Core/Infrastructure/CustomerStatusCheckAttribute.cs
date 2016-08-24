using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Validation.Models;

namespace VitalChoice.Core.Infrastructure
{
	public class CustomerStatusCheckAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
			var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
		    var userManager = context.HttpContext.RequestServices.GetService<ExtendedUserManager>();
			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				if (claimUser.HasClaim(x=>x.Type == IdentityConstants.CustomerRoleType))
				{
                    var sUserId = userManager.GetUserId(context.HttpContext.User);
                    int userId;
                    if(Int32.TryParse(sUserId, out userId))
                    {
                        var customerService = context.HttpContext.RequestServices.GetService<ICustomerService>();
                        var status = await customerService.GetCustomerStatusAsync(userId);
                        if (status == CustomerStatus.Suspended)
                        {
                            var acceptHeader = context.HttpContext.Request.Headers["Accept"];
                            if (acceptHeader.Count > 0 && acceptHeader.First().Contains("application/json"))
                            {
                                context.Result = CreateJsonResponse();
                            }
                            else
                            {
                                context.Result = new RedirectResult("/content/issues-with-account");
                            }
                        }
                    }
                }
			}
		}

	    public static IActionResult CreateJsonResponse()
	    {
	        return new JsonResult(new Result<string>(false, "/content/issues-with-account", "redirect"));
        }
	}
}