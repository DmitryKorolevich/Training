using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectViewService : IRedirectViewService
    {
        private readonly IRepositoryAsync<Redirect> _redirectRepository;

        private Dictionary<string, string> _map;
        private object _mapSyncObject = new object();

        public Dictionary<string, string> Map
        {
            get
            {
                //TODO - should be read once after adding global caching for DB
                //if (_map == null)
                //{
                //    lock (_mapSyncObject)
                //    {
                //        if (_map == null)
                //        {
                var items = _redirectRepository.Query(p => p.StatusCode == RecordStatusCode.Active).Select(false);
                _map = items.ToDictionary(p => p.From.ToLower(), x => x.To);
                //        }
                //    }
                //}
                return _map;
            }
        }

        public RedirectViewService(IRepositoryAsync<Redirect> redirectRepository)
        {
            _redirectRepository = redirectRepository;
        }

        public async Task<bool> CheckRedirects(HttpContext context)
        {
            if (context.Request.Path.HasValue)
            {
                var path = context.Request.Path.ToUriComponent()+ context.Request.QueryString.ToUriComponent();
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
