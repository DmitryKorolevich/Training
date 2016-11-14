using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectViewService : IRedirectViewService
    {
        private class RedirectOptions
        {
            public Dictionary<string, string> PathQueryMap { get; set; }

            public Dictionary<string, string> PathMap { get; set; }
        }

        private readonly IRepositoryAsync<Redirect> _redirectRepository;

        private RedirectOptions _redirectOptions
        {
            get
            {
                //will be loaded from cache
                var items = _redirectRepository.Query().Select(false);
                items = items.Where(p => p.StatusCode == RecordStatusCode.Active).ToList();

                var toReturn = new RedirectOptions();
                toReturn.PathQueryMap = items.Where(p=>!p.IgnoreQuery).ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase);
                toReturn.PathMap = items.Where(p => p.IgnoreQuery).ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase);

                return toReturn;
            }
        }

        public RedirectViewService(IRepositoryAsync<Redirect> redirectRepository)
        {
            _redirectRepository = redirectRepository;
        }

        public bool CheckRedirects(HttpContext context)
        {
            if (!context.Request.Path.HasValue)
                return false;

            var pagePath = context.Request.Path.ToUriComponent();
            if (pagePath.Equals("/shop/pc/home.asp", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Query.ContainsKey("idaffiliate"))
            {
                context.Response.Redirect($"/?idaffiliate={context.Request.Query["idaffiliate"]}", true);
                return true;
            }
            else if (pagePath.StartsWith("/category/", StringComparison.InvariantCultureIgnoreCase))
            {
                pagePath = "/products/" + pagePath.Substring("/category/".Length, pagePath.Length - "/category/".Length);
                context.Response.Redirect(pagePath + context.Request.QueryString.ToUriComponent(), true);
                return true;
            }
            else
            {
                var options = _redirectOptions;

                var queryPart = context.Request.QueryString.ToUriComponent();
                var fullPath = pagePath + queryPart;
                string redirect;

                if (!options.PathQueryMap.TryGetValue(fullPath, out redirect))
                {
                    if (!options.PathMap.TryGetValue(pagePath, out redirect))
                    {
                        return false;
                    }
                    else
                    {
                        if (redirect.Contains("?") && !string.IsNullOrEmpty(queryPart))
                        {
                            queryPart ="&"+ queryPart.Remove(0, 1);
                        }
                        context.Response.Redirect(redirect+ queryPart, true);
                        return true;
                    }
                }
                else
                {
                    context.Response.Redirect(redirect, true);
                    return true;
                }
            }
        }
    }
}