using System.Linq;
using VitalChoice.Validation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Core.GlobalFilters
{
    public class ApiModelAutoValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var acceptHeader = context.HttpContext.Request.Headers["Accept"];
            if (acceptHeader.Count > 0 && acceptHeader.First().Contains("application/json"))
            {
                if (!context.ModelState.IsValid)
                {
                    var result = new Result<object>(false);
                    foreach (var keyValue in context.ModelState)
                    {
                        if (keyValue.Value.Errors != null && keyValue.Value.Errors.Count > 0)
                            result.AddMessage(keyValue.Key,
                                keyValue.Value.Errors.First().ErrorMessage == string.Empty
                                    ? (keyValue.Value.Errors.First().Exception ?? new ApiException()).Message
                                    : keyValue.Value.Errors.First().ErrorMessage);
                    }

                    context.Result = new JsonResult(result);
                }
            }
        }
    }
}