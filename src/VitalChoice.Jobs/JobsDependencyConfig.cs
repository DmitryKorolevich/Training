using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Dnx.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Jobs.Jobs;

namespace VitalChoice.Jobs
{
    public class JobsDependencyConfig:DefaultDependencyConfig
    {
	    protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
			//builder.RegisterType<>()
			builder.RegisterModule(new QuartzAutofacFactoryModule());
			builder.RegisterModule(new QuartzAutofacJobsModule(typeof(Configuration).Assembly));
		}
    }
}
