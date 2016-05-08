using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
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
	    private void InitializeComponent()
        {
            this.ServiceName = "jobsService";
            this.CanStop = true;
        }

		private ILogger _logger;
		private readonly IContainer _container;
	    private IScheduler _scheduler;

        public JobWindowsService(IApplicationEnvironment env)
		{
            try
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile("config.json")
                    .AddJsonFile("config.local.json", true);

                var configuration = configurationBuilder.Build();

                _container = new JobsDependencyConfig().RegisterInfrastructure(configuration, new ServiceCollection(),
                    typeof(JobsDependencyConfig).Assembly, env, enableCache: true);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "Error");
                _logger.LogCritical(e.Message, e);
            }
            InitializeComponent();
		}

	    protected override void OnStart(string[] args)
	    {
	        base.OnStart(args);
	        RequestAdditionalTime(30000);
	        Trace.WriteLine("Jobs service started initialization");
            _logger = _container.Resolve<ILogger>();
            _logger.LogWarning("Scheduler start");
            var conf = _container.Resolve<IOptions<AppOptions>>().Value;
            _scheduler = _container.Resolve<IScheduler>();
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

                _scheduler.ScheduleJob(job, trigger);
            }

            _scheduler.Start();
            Trace.WriteLine("Jobs service operating normally");
	    }

	    protected override void OnStop()
        {
            base.OnStop();
            int timeout = 10000;
            var task = Task.Factory.StartNew(() =>
            {
                _scheduler.Shutdown(true);
                _container.Dispose();
                Trace.WriteLine("Jobs service stopped");
            });
            RequestAdditionalTime(timeout);
            while (!task.Wait(timeout))
            {
                RequestAdditionalTime(timeout);
            }
        }
	}
}