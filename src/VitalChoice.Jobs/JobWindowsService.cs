﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
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
	public sealed class JobWindowsService : ServiceBase
	{
        public IWebHost Host { get; set; }

        private void InitializeComponent()
        {
            this.ServiceName = "jobsService";
            this.CanStop = true;
        }

		private readonly ILogger _logger;
		private readonly IServiceProvider _container;
	    private readonly IScheduler _scheduler;

        public JobWindowsService()
		{
            try
            {
                Host = new WebHostBuilder()
                    .UseContentRoot(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]))
                    .UseStartup<Startup>()
                    .Build();

                Host.Start();

                _container = Host.Services;
                var factory = _container.GetRequiredService<ILoggerFactory>();
                _logger = factory.CreateLogger<JobWindowsService>();
                _scheduler = _container.GetRequiredService<IScheduler>();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
                throw;
            }
            InitializeComponent();
		}

	    protected override void OnStart(string[] args)
	    {
	        base.OnStart(args);
	        RequestAdditionalTime(30000);
	        EventLog.WriteEntry("Jobs service started initialization", EventLogEntryType.Information);
	        try
	        {
                _logger.LogWarning("Jobs init");
                var conf = _container.GetRequiredService<IOptions<AppOptions>>().Value;
	            var jobImpls = _container.GetServices<IJob>();
	            foreach (var impl in jobImpls)
	            {
	                var type = impl.GetType();
	                EventLog.WriteEntry($"{type} init", EventLogEntryType.Information);
                    var job = JobBuilder.Create(type).WithIdentity(type.FullName).Build();

	                var trigger = TriggerBuilder.Create()
	                    .WithIdentity(type.FullName)
	                    .WithCronSchedule(conf.JobSettings.Schedules[type.Name])
	                    .StartNow()
	                    .Build();

	                _scheduler.ScheduleJob(job, trigger);
	            }

	            _scheduler.Start();
	            EventLog.WriteEntry("Jobs service operating normally", EventLogEntryType.Information);
	        }
	        catch (Exception e)
	        {
	            _logger.LogError(e.ToString());
	            EventLog.WriteEntry($"Jobs service started with errors:\n{e}", EventLogEntryType.Error);
	        }
	    }

	    protected override void OnStop()
        {
            base.OnStop();
            int timeout = 10000;
            var task = Task.Factory.StartNew(() =>
            {
                _scheduler.Shutdown(true);
                Host.Dispose();
                EventLog.WriteEntry("Jobs service stopped", EventLogEntryType.Information);
            });
            RequestAdditionalTime(timeout);
            while (!task.Wait(timeout))
            {
                RequestAdditionalTime(timeout);
            }
        }
	}
}