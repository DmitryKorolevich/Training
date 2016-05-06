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
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Core.GlobalFilters
{
    public class MvcExceptionFilter : ExceptionFilterAttribute
	{
        private readonly ApiExceptionFilterAttribute _apiExceptionFilter = new ApiExceptionFilterAttribute();

		public override void OnException(ExceptionContext context)
		{
#if !DOTNET5_4
            var systemException = context.Exception as SystemException;
		    if (systemException != null)
		    {
		        var root = ProfilingScope.GetRootScope();
		        if (root != null)
		        {
		            root.ForceLog = true;
		        }
		    }
#endif

            var acceptHeader = context.HttpContext.Request.Headers["Accept"];
            if (acceptHeader.Any() && acceptHeader.First().Contains("application/json"))
			{
			    if (context.Exception is CustomerSuspendException)
			    {
                    context.Result = CustomerStatusCheckAttribute.CreateJsonResponse();
                    return;
			    }
                _apiExceptionFilter.OnException(context);
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
                var currentControllerName = (string) context.RouteData.Values["controller"];

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
				        foreach (var message in exception.Messages.Where(m => m.MessageType == MessageType.FormField))
				        {
				            context.ModelState.AddModelError(message.Field, message.Message);
				        }
				        foreach (var message in exception.Messages.Where(m => m.MessageType == MessageType.FieldAsCode))
				        {
				            if (message.Field == "ConcurrencyFailure")
				            {
				                SetDataChangedError(context, result, currentActionName, currentControllerName);
				            }
				            else
				            {
				                context.ModelState.AddModelError(string.Empty, message.Message);
				            }
				        }
				        result.ViewName = currentActionName;
				        result.StatusCode = (int) HttpStatusCode.OK;
				    }
				    else
				    {
				        var dbUpdateException = context.Exception as DbUpdateException;
				        if (dbUpdateException != null)
				        {
				            SetDataChangedError(context, result, currentActionName, currentControllerName);
				            var logger = LoggerService.GetDefault();
				            logger.LogError(context.Exception.Message, context.Exception);
				        }
				        else
				        {
				            result.ViewName = "Error";
				            result.StatusCode = (int) HttpStatusCode.InternalServerError;

				            var logger = LoggerService.GetDefault();
				            logger.LogError(context.Exception.Message, context.Exception);
				        }
				    }
				}
				else
				{
				    //var pageService = context.HttpContext.RequestServices.GetService<IPageResultService>();

                    if (apiException.Status == HttpStatusCode.NotFound)
					{
						context.Result = new HttpNotFoundResult();
						return;
					}
					if (apiException.Status == HttpStatusCode.Forbidden)
					{
						context.Result = new HttpForbiddenResult();
                        return;
					}
					var viewName = "Error";

					result.ViewName = viewName;
					result.StatusCode = (int)apiException.Status;

					var logger = LoggerService.GetDefault();
                    logger.LogError(context.Exception.Message, context.Exception);
                }
				context.Result = result;
			}
		}

        private static void SetDataChangedError(ExceptionContext context, ViewResult result, string currentActionName, string controllerName)
        {
            result.ViewName = currentActionName;
            result.StatusCode = (int) HttpStatusCode.OK;
            context.ModelState.AddModelError(string.Empty, "The data has been changed, please Reload page to see changes");
        }
	}
}
