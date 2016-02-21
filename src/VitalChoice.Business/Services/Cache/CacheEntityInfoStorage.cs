using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Caching.Services;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Business.Services.Cache
{
    public class CacheEntityInfoStorage : EntityInfoStorage
    {
        public CacheEntityInfoStorage(IOptions<AppOptionsBase> options, ILogger logger) : base(options, logger)
        {
        }

        protected override IEnumerable<IModel> GetCachableContextModels()
        {
            return new [] {new EcommerceContext(Options).Model, new VitalChoiceContext(Options).Model};
        }
    }
}
