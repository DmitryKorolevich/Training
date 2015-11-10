﻿using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Primitives;
using System;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Interfaces.Services.Affiliates;
using Microsoft.Extensions.Internal;
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
                        //var affiliateService = context.HttpContext.ApplicationServices.GetService<IAffiliateService>();
                        //var any = affiliateService.SelectAnyAsync(idAffiliate).Result;
                        //if (any)
                        //{
                            context.HttpContext.Response.Cookies.Append(AffiliateConstants.AffiliatePublicIdParam, idAffiliate.ToString(), new CookieOptions()
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
}