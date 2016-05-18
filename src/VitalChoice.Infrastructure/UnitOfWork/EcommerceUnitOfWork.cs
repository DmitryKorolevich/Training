using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
    {
        public EcommerceUnitOfWork(DbContextOptions<EcommerceContext> contextOptions) : base(new EcommerceContext(Options, contextOptions))
        {

        }
    }
}