using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
    {
        public EcommerceUnitOfWork() : base(new EcommerceContext(Options))
        {

        }
    }
}