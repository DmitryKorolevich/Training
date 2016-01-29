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

            var path = context.Request.Path.ToUriComponent() + context.Request.QueryString.ToUriComponent();
            path = path.ToLower();

            string redirect;
            
            if (!Map.TryGetValue(path, out redirect))
                return false;

            context.Response.Redirect(redirect, true);
            return true;
        }
    }
}