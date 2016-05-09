using System.Globalization;
using System.Net;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using System.Linq;

namespace VitalChoice.Core.Services
{
    public static class MvcExtensions
    {
        public static IApplicationBuilder UseStatusCodeExecutePath(this IApplicationBuilder app, string path,
            HttpStatusCode statusCode)
        {
            return app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == (int) statusCode && context.HttpContext.Request.Headers["Accept"].Any(h => h.ToLower().Contains("text/html")))
                {
                    PathString pathString = new PathString(path);
                    PathString originalPath = context.HttpContext.Request.Path;
                    context.HttpContext.Features.Set((IStatusCodeReExecuteFeature) new StatusCodeReExecuteFeature
                    {
                        OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                        OriginalPath = originalPath.Value
                    });
                    context.HttpContext.Request.Path = pathString;
                    try
                    {
                        await context.Next(context.HttpContext);
                    }
                    finally
                    {
                        context.HttpContext.Request.Path = originalPath;
                        context.HttpContext.Features.Set((IStatusCodeReExecuteFeature) null);
                    }
                }
            });
        }
    }

    public interface IPageResultService
    {
        IActionResult GetResult(PageResult type);
    }

    public class PageResultService : IPageResultService
    {
        public IActionResult GetResult(PageResult type)
        {
            if (type == PageResult.NotFound)
                return new RedirectResult("/content/" + ContentConstants.NOT_FOUND_PAGE_URL);
            if (type == PageResult.Forbidden)
                return new RedirectResult("/content/" + ContentConstants.ACESS_DENIED_PAGE_URL);

            throw new ApiException("Page type not implemented");
        }
    }

    public enum PageResult
    {
        NotFound,
        Forbidden
    }
}
