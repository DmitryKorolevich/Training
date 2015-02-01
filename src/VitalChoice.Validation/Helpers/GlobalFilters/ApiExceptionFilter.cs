using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Models;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            return base.OnExceptionAsync(context);
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