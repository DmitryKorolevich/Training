using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Validation.Models;
using Microsoft.AspNet.Mvc.Filters;

namespace VitalChoice.Core.GlobalFilters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            JsonResult result;
            var apiException = context.Exception as ApiException;
            if (apiException == null)
            {
                var exception = context.Exception as AppValidationException;
                if (exception != null)
                {
                    result = new JsonResult(ResultHelper.CreateErrorResult<object>(exception.Messages))
                    {
                        StatusCode = (int) HttpStatusCode.OK
                    };
                }
                else
                {
                    result = new JsonResult(ResultHelper.CreateErrorResult<object>(ApiException.GetDefaultErrorMessage))
                    {
                        StatusCode = (int) HttpStatusCode.InternalServerError
                    };
                    LoggerService.GetDefault().LogError(context.Exception.ToString());
                }
            }
            else
            {
                result = new JsonResult(ResultHelper.CreateErrorResult<object>(apiException.Message))
                {
                    StatusCode = (int) apiException.Status
                };
            }

            context.Result = result;
        }
    }
}