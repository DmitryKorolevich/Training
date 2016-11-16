using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class RedirectJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public RedirectJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<RedirectJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started RedirectJob  job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var redirectService = scope.Resolve<IRedirectService>();
                    redirectService.ChangeRedirectsBasedOnFutureRedirectsAsync().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished RedirectJob  job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}