using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectViewService : IRedirectViewService
    {
        private readonly ILifetimeScope _rootScope;
        private static volatile RedirectData _latestData;
        private readonly BasicTimer _timer;

        private class RedirectData
        {
            public Dictionary<string, string> PathQueryMap { get; set; }

            public Dictionary<string, string> PathMap { get; set; }
        }

        private RedirectData GetRedirectData()
        {
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var redirectRepo = scope.Resolve<IRepositoryAsync<Redirect>>();
                var items = redirectRepo.Query().Select(false);
                items = items.Where(p => p.StatusCode == RecordStatusCode.Active).ToList();
                return new RedirectData
                {
                    PathQueryMap = items.Where(p => !p.IgnoreQuery)
                        .ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase),
                    PathMap = items.Where(p => p.IgnoreQuery)
                        .ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase)
                };
            }
        }

        public RedirectViewService(ILifetimeScope rootScope, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<RedirectViewService>();
            _rootScope = rootScope;
            _timer = new BasicTimer(() => _latestData = GetRedirectData(), TimeSpan.FromMinutes(5),
                exception => logger.LogError(exception.ToString()));
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
                var options = _latestData;

                var queryPart = context.Request.QueryString.ToUriComponent();
                var fullPath = pagePath + queryPart;
                string redirect;

                if (options.PathQueryMap.TryGetValue(fullPath, out redirect))
                {
                    context.Response.Redirect(redirect, true);
                    return true;
                }
                if (options.PathMap.TryGetValue(pagePath, out redirect))
                {
                    if (redirect.Contains("?") && !string.IsNullOrEmpty(queryPart))
                    {
                        queryPart = "&" + queryPart.Remove(0, 1);
                    }
                    context.Response.Redirect(redirect + queryPart, true);
                    return true;
                }
                return false;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}