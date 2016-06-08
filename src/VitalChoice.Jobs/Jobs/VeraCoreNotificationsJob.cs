using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Jobs.Jobs
{
    public class VeraCoreNotificationsJob : IJob
    {
	    private readonly IVeraCoreNotificationService _veraCoreNotificationService;
	    private readonly ILogger _logger;

		public VeraCoreNotificationsJob(IVeraCoreNotificationService veraCoreNotificationService, ILogger logger)
		{
            _veraCoreNotificationService = veraCoreNotificationService;
			_logger = logger;
		}

	    public void Execute(IJobExecutionContext context)
	    {
		    try
		    {
                _logger.LogWarning("Started VeraCoreNotificationsJob job");
                _veraCoreNotificationService.Process().GetAwaiter().GetResult();
                _logger.LogWarning("Finished VeraCoreNotificationsJob job");
			}
		    catch (Exception ex)
		    {
				_logger.LogError(0, ex, ex.Message);
				throw;
		    }
	    }
    }
}
