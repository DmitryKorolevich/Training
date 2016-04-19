using System;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class ShipDelayJob : IJob
    {
	    private readonly IOrderSchedulerService _orderSchedulerService;
	    private readonly ILogger _logger;

		public ShipDelayJob(IOrderSchedulerService orderSchedulerService, ILogger logger)
		{
            _orderSchedulerService = orderSchedulerService;
			_logger = logger;
		}

	    public void Execute(IJobExecutionContext context)
	    {
		    try
		    {
                _logger.LogWarning("Started ShipDelay job");
                _orderSchedulerService.UpdateShipDelayedOrders().GetAwaiter().GetResult();
                _logger.LogWarning("Finished ShipDelay job");
			}
		    catch (Exception ex)
		    {
				_logger.LogError(ex.Message, ex);
				throw;
		    }
	    }
    }
}
