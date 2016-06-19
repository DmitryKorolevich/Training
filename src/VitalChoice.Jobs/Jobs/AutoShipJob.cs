using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Jobs.Jobs
{
    public class AutoShipJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public AutoShipJob(ILoggerFactory logger, ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<AutoShipJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Started AutoShip job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var orderService = scope.Resolve<IOrderService>();
                    orderService.SubmitAutoShipOrders().GetAwaiter().GetResult();
                }
                Console.WriteLine("Finished AutoShip job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}