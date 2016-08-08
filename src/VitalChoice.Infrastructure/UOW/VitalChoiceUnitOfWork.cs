using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Data.UOW;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UOW
{
    public class VitalChoiceUnitOfWork : UnitOfWork
    {
        public VitalChoiceUnitOfWork(DbContextOptions<VitalChoiceContext> contextOptions, IOptions<AppOptionsBase> appOptions)
            : base(new VitalChoiceContext(appOptions, contextOptions), true)
        {

        }
    }
}