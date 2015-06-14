﻿using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime.Infrastructure;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.UnitOfWork
{
    public class VitalChoiceUnitOfWork : UnitOfWorkBase
	{
        protected override IUnitOfWorkAsync Init()
        {
            var context = new VitalChoiceContext(Options);
			return new Data.UnitOfWork.UnitOfWork(context);
		}
	}
}