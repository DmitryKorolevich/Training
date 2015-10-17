using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Validation.Models;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace VitalChoice.Core.GlobalFilters
{
    public class MvcExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			var currentActionName = (string)context.RouteData.Values["action"];

			var result = new ViewResult();
			result.ViewData = null;

			var apiException = context.Exception  as ApiException;
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
					result.StatusCode = (int) HttpStatusCode.OK;
				}
				else
				{
					result.ViewName = "Error";
                    result.StatusCode = (int) HttpStatusCode.InternalServerError;

					LoggerService.GetDefault().LogError(context.Exception.ToString());
				}
			}
			else
			{
				result.ViewName = "Error";
				result.StatusCode = (int) apiException.Status;
			}

			context.Result = result;
		}
	}
}
