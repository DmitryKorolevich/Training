using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Identity;

namespace VitalChoice.Core.GlobalFilters
{
    public class CustomerAutoReloginFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimUser = context.HttpContext.User;
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
            var result =
                await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
            if (!result)
            {
                var principal = await context.HttpContext.AuthorizeFromCookie();
                if (principal != null)
                {
                    context.HttpContext.User = principal;
                }
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}