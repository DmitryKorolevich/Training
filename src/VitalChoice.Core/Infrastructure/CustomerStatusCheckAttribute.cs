﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;
using Microsoft.AspNet.Mvc.Filters;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Core.Infrastructure
{
	public class CustomerStatusCheckAttribute : AuthorizationFilterAttribute
	{
		public override async Task OnAuthorizationAsync(AuthorizationContext context)
		{
			var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				if (claimUser.HasClaim(x=>x.Type == IdentityConstants.CustomerRoleType))
				{
                    var sUserId = context.HttpContext.User.GetUserId();
                    int userId;
                    if(Int32.TryParse(sUserId, out userId))
                    {
                        var customerService = context.HttpContext.RequestServices.GetService<ICustomerService>();
                        var status = customerService.GetCustomerStatusAsync(userId).Result;
                        if (status == CustomerStatus.Suspended)
                        {
                            context.Result = new RedirectResult("/content/issues-with-account");
                        }
                    }
                }
			}
		}
	}
}