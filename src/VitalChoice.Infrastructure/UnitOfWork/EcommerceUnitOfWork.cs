using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
	{
		protected override IUnitOfWorkAsync Init()
	    {
			var context = new EcommerceContext();
			return new Data.UnitOfWork.UnitOfWork(context);
		}
	}
}