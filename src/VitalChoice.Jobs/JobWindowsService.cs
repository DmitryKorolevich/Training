using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Quartz;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Jobs
{
	public class JobWindowsService : ServiceBase
	{
		private readonly IOptions<AppOptions> _options;
		private readonly ILogger _logger;
		private IContainer _container;

		public JobWindowsService(IApplicationEnvironment env)
		{
			try
			{
				Trace.Listeners.Add(new EventLogTraceListener(new EventLog("Application")
				{
					Source = "JobsWindowsService"
				}));

				var configurationBuilder = new ConfigurationBuilder()
					.AddJsonFile("config.json")
					.AddJsonFile("config.local.json", true);

				var configuration = configurationBuilder.Build();

				_container = new JobsDependencyConfig().RegisterInfrastructure(configuration, new ServiceCollection(),
					typeof (JobsDependencyConfig).Assembly, env, enableCache:false);

				_logger = _container.Resolve<ILogger>();
                _logger.LogWarning("Scheduler start");
                var conf = _container.Resolve<IOptions<AppOptions>>().Value;
				var scheduler = _container.Resolve<IScheduler>();
				var jobImpls = _container.Resolve<IEnumerable<IJob>>();
				foreach (var impl in jobImpls)
				{
					var type = impl.GetType();
					var job = JobBuilder.Create(type).WithIdentity(type.FullName).Build();

					var trigger = TriggerBuilder.Create()
						.WithIdentity(type.FullName)
						.WithCronSchedule(conf.JobSettings.Schedules[type.Name])
						.StartNow()
						.Build();

					scheduler.ScheduleJob(job, trigger);
				}

				scheduler.Start();

			}
			catch (Exception e)
			{
				_logger.LogCritical(e.Message, e);
				throw;
			}
		}

		protected override void OnStart(string[] args)
		{
			base.OnStart(args);
			Trace.WriteLine("Jobs service started");
		}

		protected override void OnStop()
		{
			_container.Dispose();
			base.OnStop();
			Trace.WriteLine("Jobs service stopped");
		}
	}
}
