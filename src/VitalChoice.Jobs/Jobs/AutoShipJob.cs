using System;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Jobs.Jobs
{
    public class AutoShipJob: IJob
    {
	    private readonly IOrderService _orderService;
	    private readonly ILogger _logger;

		public AutoShipJob(IOrderService orderService, ILogger logger)
		{
			_orderService = orderService;
			_logger = logger;
		}

	    public void Execute(IJobExecutionContext context)
	    {
		    try
		    {
				Console.WriteLine("Started AutoShip job");
				_orderService.SubmitAutoShipOrders().Wait();
				Console.WriteLine("Finished AutoShip job");
			}
		    catch (Exception ex)
		    {
				_logger.LogError(ex.Message, ex);
				throw;
		    }
	    }
    }
}
