using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class VitalChoiceUnitOfWork : UnitOfWorkBase
	{
        public VitalChoiceUnitOfWork(DbContextOptions<VitalChoiceContext> contextOptions) : base(new VitalChoiceContext(Options, contextOptions))
        {
            
        }
    }
}