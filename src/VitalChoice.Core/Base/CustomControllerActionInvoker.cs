using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Core.Base
{
    public class CustomControllerActionInvoker : ControllerActionInvoker
    {
        public CustomControllerActionInvoker(ActionContext actionContext, ControllerActionInvokerCache controllerActionInvokerCache,
            IControllerFactory controllerFactory, ControllerActionDescriptor descriptor, IReadOnlyList<IInputFormatter> inputFormatters,
            IControllerActionArgumentBinder argumentBinder, IReadOnlyList<IModelValidatorProvider> modelValidatorProviders,
            IReadOnlyList<IValueProviderFactory> valueProviderFactories,
            ILogger logger, DiagnosticSource diagnosticSource, int maxModelValidationErrors)
            : base(
                actionContext, controllerActionInvokerCache, controllerFactory, descriptor, inputFormatters, argumentBinder,
                modelValidatorProviders, valueProviderFactories, logger, diagnosticSource, maxModelValidationErrors)
        {
        }

        protected override Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            var baseController = actionExecutingContext.Controller as BaseController;
            baseController?.Configure();
            return base.InvokeActionAsync(actionExecutingContext);
        }
    }
}