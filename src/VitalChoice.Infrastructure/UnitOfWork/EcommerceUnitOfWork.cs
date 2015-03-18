using System;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class EcommerceUnitOfWork : UnitOfWorkBase
	{
		protected override IUnitOfWorkAsync Init()
	    {
			var context = new VitalChoiceContext();
			return new Data.UnitOfWork.UnitOfWork(context);
		}
	}
}