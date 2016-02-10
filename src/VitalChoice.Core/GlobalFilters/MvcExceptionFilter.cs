using System.Linq;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Ecommerce.Domain.Exceptions;

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
                    result.ViewName = apiException.Status == HttpStatusCode.NotFound ? "Error404" : "Error";
					result.StatusCode = (int)apiException.Status;

                    var logger = LoggerService.GetDefault();
                    logger.LogError(context.Exception.Message, context.Exception);
                }
				context.Result = result;
			}
		}
	}
}
