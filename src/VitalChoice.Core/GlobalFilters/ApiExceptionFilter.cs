using System.Net;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Validation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ApiExceptionFilterAttribute>();
                    logger.LogError(0, context.Exception, context.Exception.Message);
                }
            }
            else
            {
                var exception = context.Exception as AccessDeniedException;
                if (exception != null)
                {
                    result = new ForbiddenResult();
                }
                else
                {
                    if ((context.Exception as NotFoundException) != null)
                    {
                        result = new HttpNotFoundResult();
                    }
                    else
                    {
                        result = new JsonResult(ResultHelper.CreateErrorResult<object>(apiException.Message))
                        {
                            StatusCode = (int)apiException.Status
                        };
                    }
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ApiExceptionFilterAttribute>();
                    if (context.Exception.InnerException != null)
                    {
                        logger.LogError(0, context.Exception.InnerException, context.Exception.InnerException.Message);
                    }
                }
            }

            context.Result = result;
        }
    }
}