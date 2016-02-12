using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
    {
        public EcommerceUnitOfWork() : base(new EcommerceContext(Options))
        {

        }

        public EcommerceUnitOfWork(EcommerceContext context) : base(context)
        {

        }
    }
}