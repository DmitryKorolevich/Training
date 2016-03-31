using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Validation.Models;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Data.Entity;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Core.GlobalFilters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            IActionResult result;
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
                    var dbUpdateException = context.Exception as DbUpdateException;
                    if (dbUpdateException != null)
                    {
                        result =
                            new JsonResult(
                                ResultHelper.CreateErrorResult<object>("The data has been changed, please Reload page to see changes"))
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
                    }
                    LoggerService.GetDefault().LogError(context.Exception.ToString());
                }
            }
            else
            {
                var exception = context.Exception as AccessDeniedException;
                if (exception != null)
                {
                    result = new HttpForbiddenResult();
                }
                else
                {
                    result = new JsonResult(ResultHelper.CreateErrorResult<object>(apiException.Message))
                    {
                        StatusCode = (int)apiException.Status
                    };
                }
            }

            context.Result = result;
        }
    }
}