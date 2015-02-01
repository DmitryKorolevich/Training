using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Models;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class ModelAutoValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        //HttpActionExecutedContext actionExecutedContext)
        //{
        //    if (!actionExecutedContext.ActionContext.ModelState.IsValid)
        //    {
        //        var result = new Result<object>(false);
        //        foreach (KeyValuePair<string, ModelState> keyValue in actionExecutedContext.ActionContext.ModelState)
        //        {
        //            if (keyValue.Value.Errors != null && keyValue.Value.Errors.Count > 0)
        //                result.AddMessage(keyValue.Key, keyValue.Value.Errors.First().ErrorMessage == string.Empty ? (keyValue.Value.Errors.First().Exception ?? new ApiException()).Message : keyValue.Value.Errors.First().ErrorMessage);
        //        }

        //        actionExecutedContext.ActionContext.Response =
        //            actionExecutedContext.ActionContext.Request.CreateResponse(HttpStatusCode.OK, result);
        //    }
        //}
    }
}