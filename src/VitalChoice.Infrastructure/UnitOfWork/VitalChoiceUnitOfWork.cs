using VitalChoice.Ecommerce.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class VitalChoiceUnitOfWork : UnitOfWorkBase
	{
        public VitalChoiceUnitOfWork() : base(new VitalChoiceContext(Options))
        {
            
        }
	}
}