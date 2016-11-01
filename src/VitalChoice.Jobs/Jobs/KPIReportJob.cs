using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.VeraCore;

namespace VitalChoice.Jobs.Jobs
{
    public class KPIReportJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public KPIReportJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<KPIReportJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started KPIReportJob job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var orderReportService = scope.Resolve<IOrderReportService>();
                    orderReportService.CreateKPIReportAsync().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished KPIReportJob job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}