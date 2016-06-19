using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
#if !NETSTANDARD1_5
            if (ProfilingScope.GetRootScope() == null)
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
                            scope.Stop();
                            _request.OnFinishedRequest(scope);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                        }
                    }
                }
                try
                {
                    var root = ProfilingScope.GetRootScope();
                    root?.Dispose();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
            else
            {
                await _requestDelegate.Invoke(context);
            }
#else
            await _requestDelegate.Invoke(context);
#endif
        }
    }
}