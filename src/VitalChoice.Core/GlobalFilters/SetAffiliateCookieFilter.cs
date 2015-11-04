using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Primitives;
using System;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Interfaces.Services.Affiliates;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;

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
                        context.HttpContext.Response.Cookies.Append(AffiliateConstants.AffiliatePublicIdParam, idAffiliate.ToString(), new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(AffiliateConstants.AffiliatePublicIdParamExpiredDays),
                        });
                    }
                }
            }

            base.OnActionExecuted(context);
        }
    }
}