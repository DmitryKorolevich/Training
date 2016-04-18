using System;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class GoogleProductsFeedJob : IJob
    {
	    private readonly IProductService _productService;
	    private readonly ILogger _logger;

		public GoogleProductsFeedJob(IProductService productService, ILogger logger)
		{
            _productService = productService;
			_logger = logger;
		}

	    public void Execute(IJobExecutionContext context)
	    {
		    try
		    {
                _logger.LogWarning("Started GoogleProductsFeed job");
                _productService.UpdateSkuGoogleItemsReportFile().GetAwaiter().GetResult();
                _logger.LogWarning("Finished GoogleProductsFeed job");
			}
		    catch (Exception ex)
		    {
				_logger.LogError(ex.Message, ex);
				throw;
		    }
	    }
    }
}
