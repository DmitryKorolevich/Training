using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.User;
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
        private readonly Dictionary<string, string> _map;

        //BUG: this query will never be cached, and even not needed
        //if (_map == null)
        //{
        //    lock (_mapSyncObject)
        //    {
        //        if (_map == null)
        //        {
        //var items = _redirectRepository.Query(p => p.StatusCode == RecordStatusCode.Active).Select(false);
        //_map = items.ToDictionary(p => p.From.ToLower(), x => x.To);
        //        }
        //    }
        //}

        public RedirectViewService(IOptions<AppOptions> options)
        {
            using (var context = new VitalChoiceContext(options))
            {
                var rep = new ReadRepositoryAsync<Redirect>(context);
                _map = rep.Query(p => p.StatusCode == RecordStatusCode.Active)
                    .Select(false)
                    .ToDictionary(p => p.From.ToLower(), x => x.To);
            }
        }

        //BUG: Make no sense to have fully sync method asyncronous
        public bool CheckRedirects(HttpContext context)
        {
            if (!context.Request.Path.HasValue)
                return false;

            var path = context.Request.Path.ToUriComponent() + context.Request.QueryString.ToUriComponent();
            path = path.ToLower();
            //BUG: double key scan !!
            //if (_map.ContainsKey(path))
            //{
            //    context.Response.Redirect(_map[path], true);
            //    return true;
            //}
            string redirect;

            //Single key scan !!
            if (!_map.TryGetValue(path, out redirect))
                return false;

            context.Response.Redirect(redirect, true);
            return true;
        }
    }
}