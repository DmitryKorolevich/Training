using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;

namespace VitalChoice.Business.Services.Impl
{
	public class SettingService : ISettingService
    {
        protected IConfiguration Configuration { get; private set; }

        public SettingService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetProjectConstant(string key)
        {
            return Configuration.Get("Constants:" + key);
        }
    }
}
