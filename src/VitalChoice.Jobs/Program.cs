using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Quartz;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Jobs.Jobs;

namespace VitalChoice.Jobs
{
	public class Program
	{
		private readonly IApplicationEnvironment _env;

		public Program(IApplicationEnvironment env)
		{
			_env = env;
		}

		public void Main(string[] args)
		{
			var servicesToRun = new ServiceBase[]
			{
				new JobWindowsService(_env)
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}
