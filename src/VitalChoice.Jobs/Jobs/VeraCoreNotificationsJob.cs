using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Jobs.Jobs
{
    public class VeraCoreNotificationsJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public VeraCoreNotificationsJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<VeraCoreNotificationsJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started VeraCoreNotificationsJob job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var veraCoreNotificationService = scope.Resolve<IVeraCoreNotificationService>();
                    veraCoreNotificationService.Process().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished VeraCoreNotificationsJob job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}