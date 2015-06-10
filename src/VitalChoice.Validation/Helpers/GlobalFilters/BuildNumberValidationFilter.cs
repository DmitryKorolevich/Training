﻿using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Entities.Options;

namespace VitalChoice.Validation.Helpers.GlobalFilters
{
    public class BuildNumberValidationFilter : ActionFilterAttribute
    {
	    public override void OnActionExecuting(ActionExecutingContext context)
	    {
			var optionsAccessor = context.HttpContext.ApplicationServices.GetService<IOptions<AppOptions>>();

			var buildNumber = context.HttpContext.Request.Headers["Build-Number"];
		    if (string.IsNullOrWhiteSpace(buildNumber) || !buildNumber.Equals(optionsAccessor.Options.Versioning.BuildNumber))
		    {
			    context.Result = new NoContentResult();
		    }
	    }
    }
}