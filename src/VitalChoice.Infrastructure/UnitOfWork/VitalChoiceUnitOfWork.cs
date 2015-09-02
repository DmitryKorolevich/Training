using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Options;
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