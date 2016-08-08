using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UOW
{
    public class EcommerceUnitOfWork : Data.UOW.UnitOfWork
    {
        public EcommerceUnitOfWork(DbContextOptions<EcommerceContext> contextOptions, IOptions<AppOptionsBase> appOptions)
            : base(new EcommerceContext(appOptions, contextOptions), true)
        {

        }
    }
}