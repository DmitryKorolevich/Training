using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Quartz;
using VitalChoice.Jobs.Jobs;

namespace VitalChoice.Jobs
{
	public class Program : ServiceBase
	{
		private readonly IApplicationEnvironment _env;

		//public Program()
		//{
		//	//_env = env;
		//}

		public Program(IApplicationEnvironment env)
		{
			_env = env;
		}

		public void Main(string[] args)
		{
			if (args.Contains("--windows-service"))
			{
				Run(this);
				return;
			}

			OnStart(null);

			//ServiceBase[] ServicesToRun;
			//ServicesToRun = new ServiceBase[]
			//{
			//	new Program()
			//};
			//ServiceBase.Run(ServicesToRun);

			var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true);

			var configuration = configurationBuilder.Build();

			var container = new JobsDependencyConfig().RegisterInfrastructure(configuration, new ServiceCollection(), typeof(Configuration).Assembly, _env);

			var job = JobBuilder.Create<AutoShipJob>()
				.WithIdentity("AutoShipJob", "job")
				.Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity("SampleTrigger", "job")
				.WithCronSchedule("0/10 * * * * ?") // #5
				.ForJob(job.Key)
				//.StartNow()
				.Build();

			var scheduler = container.GetService<IScheduler>();
			scheduler.ScheduleJob(job, trigger);
			scheduler.Start();

			Console.ReadLine();
			OnStop();
		}
	}
}
