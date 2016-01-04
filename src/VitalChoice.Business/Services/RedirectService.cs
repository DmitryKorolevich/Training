using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IReadRepositoryAsync<Redirect> _redirectRepositoryAsync;

        private Dictionary<string, string> _map;
        private object _mapSyncObject = new object();

        public Dictionary<string, string> Map
        {
            get {
                //if (_map == null)
                //{
                //    lock (_mapSyncObject)
                //    {
                //        if (_map == null)
                //        {
                            var items = _redirectRepositoryAsync.Query(p => p.StatusCode == RecordStatusCode.Active).Select(false);
                            _map = items.ToDictionary(p => p.From.ToLower(), x => x.To);
                //        }
                //    }
                //}
                return _map;
            }
        }

        public RedirectService(IReadRepositoryAsync<Redirect> redirectRepositoryAsync)
        {
            _redirectRepositoryAsync = redirectRepositoryAsync;
        }

        public async Task<bool> CheckRedirects(HttpContext context)
        {
            if (context.Request.Path.HasValue)
            {
                var path = context.Request.Path.Value.ToLower();
                if (Map.ContainsKey(path))
                {
                    context.Response.Redirect(Map[path], true);
                    return true;
                }
            }
            return false;
        }
    }
}
