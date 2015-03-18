using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class VitalChoiceUnitOfWork : UnitOfWorkBase
	{
		protected override IUnitOfWorkAsync Init()
		{
			var context = new VitalChoiceContext();
			return new Data.UnitOfWork.UnitOfWork(context);
		}
	}
}