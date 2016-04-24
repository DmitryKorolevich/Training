using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using VitalChoice.Profiling.Interfaces;

namespace VitalChoice.Profiling.Base
{
    public class ProfilerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IPerformanceRequest _request;
        private readonly ILogger _logger;

        public ProfilerMiddleware(RequestDelegate requestDelegate, IPerformanceRequest request, ILoggerFactory loggerFactory)
        {
            _request = request;
            _requestDelegate = requestDelegate;
            _logger = loggerFactory.CreateLogger<ProfilerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            using (var scope = new ProfilingScope(context.Request.Path + context.Request.QueryString))
            {
                try
                {
                    await _requestDelegate.Invoke(context);
                }
                finally
                {
                    try
                    {
#if !DOTNET5_4
                        if (scope.GetStackCount() == 1)
                        {
                            scope.Stop();
                            _request.OnFinishedRequest(scope);
                        }
#endif
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message, e);
                    }
                }
            }
        }
    }
}