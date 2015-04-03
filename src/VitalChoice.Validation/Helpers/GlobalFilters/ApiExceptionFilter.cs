﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Exceptions;

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
                    var exc = context.Exception as ApiException;
                    result = new JsonResult(Result.CreateErrorResult<object>(exc.Message));
                    result.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    result = new JsonResult(Result.CreateErrorResult<object>(ApiException.GetDefaultErrorMessage));
                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }

            context.Result = result;
        }

        //    {
    //        if (context.Exception is ApiException)
    //        {
    //            var exc = context.Exception as ApiException;
    //            context.Response =  context.Request.CreateResponse
    //                                         (exc.Status, Result.CreateErrorResult<object>(exc.Message));
    //        }
    //        else
    //        {
    //            if (context.Exception is ValidationException)
    //            {
    //                context.Response = context.Request.CreateResponse
    //                         (HttpStatusCode.OK, Result.CreateErrorResult<object>(CommonManager.LocalizationData.GetString(context.Exception.Message)));
    //            }
    //            else
    //            {
    //                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, Result.CreateErrorResult<object>
    //                                              (ApiException.GetDefaultErrorMessage));
    //            }
    //        }
    //        if (context.Request.RequestUri.Query.Contains(HeaderAndQueryStringConstants.PARAM_ERROR_RENDER_TYPE) &&
    //context.Request.RequestUri.ParseQueryString()[HeaderAndQueryStringConstants.PARAM_ERROR_RENDER_TYPE] ==
    //HeaderAndQueryStringConstants.ERROR_RENDER_TYPE_HTML)
    //        {
    //            context.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
    //        }
    //    }
    }
}