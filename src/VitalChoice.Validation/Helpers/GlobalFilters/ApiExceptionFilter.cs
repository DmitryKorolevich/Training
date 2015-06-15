using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Validation.Models;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
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
                if (context.Exception is ApiValidationException)
                {
                    var exception = context.Exception as ApiValidationException;
                    result = new JsonResult(ResultHelper.CreateErrorResult<object>(exception.Messages))
                    {
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    result = new JsonResult(ResultHelper.CreateErrorResult<object>(apiException.Message))
                    {
                        StatusCode = (int) apiException.Status
                    };
                }
            }

            context.Result = result;
        }
    }
}