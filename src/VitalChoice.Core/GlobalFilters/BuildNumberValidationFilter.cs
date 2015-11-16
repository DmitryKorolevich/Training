using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Primitives;
using VitalChoice.Infrastructure.Domain.Options;

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