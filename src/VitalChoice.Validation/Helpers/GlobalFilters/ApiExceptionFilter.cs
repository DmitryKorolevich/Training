using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Business.Services.Impl;
using Microsoft.Framework.Logging;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            JsonResult result = null;
            if (context.Exception is ApiException)
            {
                var exc = context.Exception as ApiException;
                result = new JsonResult(Result.CreateErrorResult<object>(exc.Message));
                result.StatusCode = (int)exc.Status;
            }
            else
            {
                if (context.Exception is AppValidationException)
                {
                    var exc = context.Exception as AppValidationException;
                    result = new JsonResult(Result.CreateErrorResult<object>(new MessageInfo()
                    {
                        Field = exc.Field,
                        Message= exc.Message
                    }));
                    result.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    result = new JsonResult(Result.CreateErrorResult<object>(ApiException.GetDefaultErrorMessage));
                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                    LoggerService.GetDefault().LogError(context.Exception.ToString());
                }
            }

            context.Result = result;
        }
    }
}