using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class PostShipProductReviewJob : IJob
    {
	    private readonly IOrderSchedulerService _orderSchedulerService;
	    private readonly ILogger _logger;

		public PostShipProductReviewJob(IOrderSchedulerService orderSchedulerService, ILogger logger)
		{
            _orderSchedulerService = orderSchedulerService;
			_logger = logger;
		}

	    public void Execute(IJobExecutionContext context)
	    {
		    try
		    {
                _logger.LogWarning("Started PostShipProductReview job");
                _orderSchedulerService.SendOrderProductReviewEmails().GetAwaiter().GetResult();
                _logger.LogWarning("Finished PostShipProductReview job");
			}
		    catch (Exception ex)
		    {
				_logger.LogError(0, ex, ex.Message);
				throw;
		    }
	    }
    }
}
