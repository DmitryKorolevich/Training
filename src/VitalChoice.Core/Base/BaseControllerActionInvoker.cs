using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;
using VitalChoice.Interfaces.Services;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.AspNet.Mvc.Infrastructure;
using System.Diagnostics.Tracing;

namespace VitalChoice.Core.Base
{
    public class BaseControllerActionInvoker : ControllerActionInvoker
    {
        public BaseControllerActionInvoker(
            ActionContext actionContext,
            IReadOnlyList<IFilterProvider> filterProviders,
            IControllerFactory controllerFactory,
            ControllerActionDescriptor descriptor,
            IReadOnlyList<IInputFormatter> inputFormatters,
            IReadOnlyList<IOutputFormatter> outputFormatters,
            IControllerActionArgumentBinder controllerActionArgumentBinder,
            IReadOnlyList<IModelBinder> modelBinders,
            IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
            IReadOnlyList<IValueProviderFactory> valueProviderFactories,
            IActionBindingContextAccessor actionBindingContextAccessor,
            ILogger logger,
            TelemetrySource telemetry,
            int maxModelValidationErrors)
            : base(
                actionContext, filterProviders, controllerFactory, descriptor, inputFormatters, outputFormatters,
                controllerActionArgumentBinder, modelBinders, modelValidatorProviders, valueProviderFactories,
                actionBindingContextAccessor, logger, telemetry, maxModelValidationErrors)
        {

        }

        protected override async Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            var baseController = actionExecutingContext.Controller as BaseController;
            baseController?.Configure();
            return await base.InvokeActionAsync(actionExecutingContext);
        }
    }
}