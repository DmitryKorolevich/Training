using System;
using Autofac;
using Microsoft.Extensions.Logging;
using Quartz;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Jobs.Jobs
{
    public class GoogleProductsFeedJob : IJob
    {
        private readonly ILifetimeScope _rootScope;
        private readonly ILogger _logger;

        public GoogleProductsFeedJob(ILifetimeScope rootScope, ILoggerFactory logger)
        {
            _rootScope = rootScope;
            _logger = logger.CreateLogger<GoogleProductsFeedJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogWarning("Started GoogleProductsFeed job");
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var productService = scope.Resolve<IProductService>();
                    productService.UpdateSkuGoogleItemsReportFile().GetAwaiter().GetResult();
                }
                _logger.LogWarning("Finished GoogleProductsFeed job");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}