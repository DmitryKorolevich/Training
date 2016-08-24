using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Identity;

namespace VitalChoice.Core.GlobalFilters
{
    public class SetAffiliateCookieFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var cookies = context.HttpContext.Request.Cookies[AffiliateConstants.AffiliatePublicIdParam];
            if (String.IsNullOrEmpty(cookies))
            {
                var param = context.HttpContext.Request.Query[AffiliateConstants.AffiliatePublicIdParam];
                if (!String.IsNullOrEmpty(param))
                {
                    int idAffiliate = 0;
                    if (Int32.TryParse(param, out idAffiliate))
                    {
                        //var affiliateService = context.HttpContext.ApplicationServices.GetService<IAffiliateService>();
                        //var any = affiliateService.SelectAnyAsync(idAffiliate).Result;
                        //if (any)
                        //{
                        context.HttpContext.Response.Cookies.Append(AffiliateConstants.AffiliatePublicIdParam, idAffiliate.ToString(),
                            new CookieOptions()
                            {
                                Expires = DateTime.Now.AddDays(AffiliateConstants.AffiliatePublicIdParamExpiredDays),
                            });
                        //}
                    }
                }
            }

            base.OnActionExecuted(context);
        }
    }

    public class CustomerAutoReloginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var claimUser = context.HttpContext.User;
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
            var result =
                authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile).GetAwaiter().GetResult();
            if (!result)
            {
                context.HttpContext.AuthorizeFromCookie().GetAwaiter().GetResult();
            }
            base.OnActionExecuted(context);
        }
    }
}