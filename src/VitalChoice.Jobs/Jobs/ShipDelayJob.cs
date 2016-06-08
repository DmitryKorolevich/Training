using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class ShipDelayJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public ShipDelayJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<ShipDelayJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started ShipDelay job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var orderSchedulerService = scope.Resolve<IOrderSchedulerService>();
                    orderSchedulerService.UpdateShipDelayedOrders().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished ShipDelay job");
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw;
            }
        }
    }
}