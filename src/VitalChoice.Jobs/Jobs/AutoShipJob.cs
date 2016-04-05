using System;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Jobs.Jobs
{
    public class AutoShipJob: IJob
    {
	    private readonly ICountryService _orderService;

	    public AutoShipJob(ICountryService orderService)
	    {
		    _orderService = orderService;
	    }

	    public void Execute(IJobExecutionContext context)
	    {
		    throw new NotImplementedException();
	    }
    }
}
