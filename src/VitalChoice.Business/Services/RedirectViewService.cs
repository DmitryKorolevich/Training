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
        private readonly IRepositoryAsync<Redirect> _redirectRepository;

        public Dictionary<string, string> Map
        {
            get
            {
                //will be loaded from cache
                var items = _redirectRepository.Query().Select(false);
                items = items.Where(p => p.StatusCode == RecordStatusCode.Active).ToList();
                return items.ToDictionary(p => p.From.ToLower(), x => x.To);
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
            if (pagePath == "/shop/pc/home.asp" &&
                context.Request.Query.ContainsKey("idaffiliate"))
            {
                context.Response.Redirect($"/?idaffiliate={context.Request.Query["idaffiliate"]}", true);
                return true;
            }
            else if (pagePath.StartsWith("/category/"))
            {
                pagePath = "/products/" + pagePath.Substring("/category/".Length, pagePath.Length - "/category/".Length);
                context.Response.Redirect(pagePath + context.Request.QueryString.ToUriComponent(), true);
                return true;
            }
            else
            {
                var path = pagePath + context.Request.QueryString.ToUriComponent();
                path = path.ToLower();
                string redirect;

                if (!Map.TryGetValue(path, out redirect))
                    return false;

                context.Response.Redirect(redirect, true);
                return true;
            }
        }
    }
}