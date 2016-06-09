using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Validation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Templates.Strings;
using Templates.Strings.Core;
//using VitalChoice.Caching.Debug;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache.Base;
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
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<ApiExceptionFilterAttribute>();
                    var dbUpdateException = context.Exception as DbUpdateException;
                    if (dbUpdateException != null)
                    {
                        result =
                            new JsonResult(
                                ResultHelper.CreateErrorResult<object>("The data has been changed, please Reload page to see changes"))
                            {
                                StatusCode = (int) HttpStatusCode.OK
                            };
                        logger.LogError(0, FormatUpdateException(context, dbUpdateException));
                    }
                    else
                    {
                        result = new JsonResult(ResultHelper.CreateErrorResult<object>(ApiException.GetDefaultErrorMessage))
                        {
                            StatusCode = (int) HttpStatusCode.InternalServerError
                        };
                        logger.LogError(0, context.Exception.ToString());
                    }
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
                        result = new NotFoundResult();
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
                        logger.LogError(0, context.Exception.ToString());
                    }
                }
            }

            context.Result = result;
        }

        internal static string FormatUpdateException(ExceptionContext context, DbUpdateException dbUpdateException)
        {
            //var updateIssues = CacheDebugger.ProcessDbUpdateException(dbUpdateException);
            ExStringBuilder builder = new ExStringBuilder(context.Exception.ToString());
            builder += "\nTrace Data:";
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTime
            });

            //foreach (var cacheUpdateData in updateIssues)
            //{
            //    WriteObject(builder, "Entity Type", jsonSerializer, cacheUpdateData.EntityType.ToString());
            //    WriteObject(builder, "Updated Object", jsonSerializer, cacheUpdateData.UpdateEntity);
            //    WriteObject(builder, "Cache Variants", jsonSerializer, cacheUpdateData.CachedEntities);
            //    WriteObject(builder, "Actual DB Data", jsonSerializer, cacheUpdateData.ActualDbEntity);
            //}
            return builder.ToString();
        }

        private static void WriteObject(ExStringBuilder builder, string preText, JsonSerializer jsonSerializer,
            object data, string postText = null)
        {
            builder += $"\n\t{preText}:";
            var stringWriter = new ExStringWriter();
            using (new JsonTextWriter(stringWriter))
            {
                jsonSerializer.Serialize(stringWriter, data);
            }
            builder += stringWriter.ToString();
            if (!string.IsNullOrEmpty(postText))
            {
                builder.Append(postText);
            }
        }
    }
}