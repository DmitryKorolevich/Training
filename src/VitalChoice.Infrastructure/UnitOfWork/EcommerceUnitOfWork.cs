using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime.Infrastructure;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
	{
        protected override IUnitOfWorkAsync Init()
	    {
            var options = CallContextServiceLocator.Locator.ServiceProvider.GetService<IOptions<AppOptions>>();
            var context = new EcommerceContext(options);
			return new Data.UnitOfWork.UnitOfWork(context);
		}
	}
}