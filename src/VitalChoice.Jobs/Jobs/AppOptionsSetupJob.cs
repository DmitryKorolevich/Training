using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Jobs.Jobs
{
    public class AppOptionsSetupJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public AppOptionsSetupJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<AppOptionsSetupJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started AppOptionsSetupJob job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var appInfrastructureService = scope.Resolve<IAppInfrastructureService>();
                    appInfrastructureService.SetupAppSettings().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished AppOptionsSetupJob job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}