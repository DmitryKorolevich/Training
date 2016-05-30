using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
		private readonly IServiceProvider _container;
	    private IScheduler _scheduler;

        public JobWindowsService()
		{
            try
            {
                _container = Program.Host.Services;
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
            _logger = _container.GetRequiredService<ILogger>();
            _logger.LogWarning("Scheduler start");
            var conf = _container.GetRequiredService<IOptions<AppOptions>>().Value;
            _scheduler = _container.GetRequiredService<IScheduler>();
            var jobImpls = _container.GetRequiredService<IEnumerable<IJob>>();
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
                Program.Host.Dispose();
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