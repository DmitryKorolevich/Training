using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Infrastructure;

namespace VitalChoice.Caching.Extensions
{
    public class DbContextCacheExtension : IDbContextOptionsExtension
    {
        public void ApplyServices(EntityFrameworkServicesBuilder builder)
        {
            builder.AddEntityFrameworkCache();
        }
    }
}
