using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Core.GlobalFilters
{
    public class BuildNumberValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller?.GetType().GetCustomAttribute(typeof(IgnoreBuildNumberAttribute)) != null)
            {
                return;
            }
            var controllerDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerDescriptor?.MethodInfo.GetCustomAttribute(typeof(IgnoreBuildNumberAttribute)) != null)
            {
                return;
            }

            var optionsAccessor = context.HttpContext.RequestServices.GetService<IOptions<AppOptions>>();

            var buildNumber = context.HttpContext.Request.Headers["Build-Number"];
            if (StringValues.IsNullOrEmpty(buildNumber))
            {
                buildNumber = context.HttpContext.Request.Query["buildNumber"];
            }

            if (StringValues.IsNullOrEmpty(buildNumber) ||
                !buildNumber.ToString().Equals(optionsAccessor.Value.Versioning.BuildNumber))
            {
                context.Result = new NoContentResult();
            }
        }
    }
}