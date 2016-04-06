using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Jobs.Infrastructure;

namespace VitalChoice.Jobs
{
    public class JobsDependencyConfig:DefaultDependencyConfig
    {
	    protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
			//required by our buisness layer which is bad(
		    builder.RegisterType<DummyHttpContextAccessor>().As<IHttpContextAccessor>();

			builder.RegisterModule(new QuartzAutofacFactoryModule());
			builder.RegisterModule(new QuartzAutofacJobsModule(typeof(JobsDependencyConfig).Assembly));
		}

	    protected override void ConfigureAppOptions(IConfiguration configuration, AppOptions options)
	    {
		    base.ConfigureAppOptions(configuration, options);

		    var items = configuration.GetSection("App:JobSettings:Schedules").GetChildren();

		    options.JobSettings = new JobSettings()
		    {
			    Schedules = items.ToDictionary(x=>x.Key, y=>y.Value)
		    };
	    }
	}
}
