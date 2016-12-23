using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectViewService : IRedirectViewService
    {
        private static readonly object LockObj = new object();

        private static volatile RedirectData _latestData;

        private static object _nextUpdateDate = DateTime.MinValue;

        private class RedirectData
        {
            public Dictionary<string, string> PathQueryMap { get; set; }

            public Dictionary<string, string> PathMap { get; set; }
        }

        private readonly IRepositoryAsync<Redirect> _redirectRepository;

        private RedirectData GetRedirectData()
        {
            var now = DateTime.Now;
            if (_latestData == null || now > (DateTime) _nextUpdateDate)
            {
                lock (LockObj)
                {
                    if (_latestData == null || now > (DateTime) _nextUpdateDate)
                    {
                        _nextUpdateDate = now.AddMinutes(5);
                        var items = _redirectRepository.Query().Select(false);
                        items = items.Where(p => p.StatusCode == RecordStatusCode.Active).ToList();

                        _latestData = new RedirectData
                        {
                            PathQueryMap = items.Where(p => !p.IgnoreQuery)
                                .ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase),
                            PathMap = items.Where(p => p.IgnoreQuery)
                                .ToDictionary(p => p.From, x => x.To, StringComparer.InvariantCultureIgnoreCase)
                        };
                    }
                }
            }
            return _latestData;
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
                var options = GetRedirectData();

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
    }
}