using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Primitives;
using VitalChoice.Domain.Entities.Options;

namespace VitalChoice.Core.GlobalFilters
{
    public class BuildNumberValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var optionsAccessor = context.HttpContext.ApplicationServices.GetService<IOptions<AppOptions>>();

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