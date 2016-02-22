using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Core.GlobalFilters
{
    public class MvcExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
            var acceptHeader = context.HttpContext.Request.Headers["Accept"];
            if (acceptHeader.Any() && acceptHeader.First().Contains("application/json"))
			{
				new ApiExceptionFilterAttribute().OnException(context);
			}
			else
            {
                //var referer = (string)context.HttpContext.Request.Headers["Referer"];

                //if (!string.IsNullOrWhiteSpace(referer))
                //{
                //    if (referer.StartsWith($"http://{context.HttpContext.Request.Host}") || referer.StartsWith($"https://{context.HttpContext.Request.Host}"))
                //    {
                //        var uri = new Uri(referer);
                //        var contextFactory = context.HttpContext.RequestServices.GetService<IHttpContextFactory>();
                //        //var requestFactory = context.HttpContext.RequestServices.GetService<IHttpRequestFeature>();
                //        var fakeContext = contextFactory.Create(context.HttpContext.RequestServices.GetService<IFeatureCollection>());
                //        fakeContext.Request.QueryString = new QueryString(uri.Query);
                //        fakeContext.Request.Scheme = uri.Scheme;
                //        fakeContext.Request.Host = context.HttpContext.Request.Host;
                //        fakeContext.Request.Path = new PathString(uri.AbsolutePath);
                //        QueryStringValueProviderFactory valueProviderFactory = new QueryStringValueProviderFactory();
                //        var valueProvider =
                //            (ReadableStringCollectionValueProvider)valueProviderFactory.GetValueProviderAsync(new ValueProviderFactoryContext(fakeContext, null)).Result;
                //    }
                    
                //}
                var currentActionName = (string)context.RouteData.Values["action"];

				var result = new ViewResult
				{
					ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState),
					TempData = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionary>()
				};

				var apiException = context.Exception as ApiException;
				if (apiException == null)
				{
					var exception = context.Exception as AppValidationException;
					if (exception != null)
					{
						foreach (var message in exception.Messages)
						{
							context.ModelState.AddModelError(message.Field, message.Message);
						}

						result.ViewName = currentActionName;
						result.StatusCode = (int)HttpStatusCode.OK;
					}
					else
					{
						result.ViewName = "Error";
						result.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var logger = LoggerService.GetDefault();
                        logger.LogError(context.Exception.Message, context.Exception);
                    }
				}
				else
				{
					if (apiException.Status == HttpStatusCode.NotFound)
					{
						context.Result = new RedirectResult("/content/" + ContentConstants.NOT_FOUND_PAGE_URL);
						return;
					}
					var viewName = apiException.Status == HttpStatusCode.Forbidden ? "AccessDenied" : "Error";

					result.ViewName = viewName;
					result.StatusCode = (int)apiException.Status;

					var logger = LoggerService.GetDefault();
                    logger.LogError(context.Exception.Message, context.Exception);
                }
				context.Result = result;
			}
		}
	}
}
