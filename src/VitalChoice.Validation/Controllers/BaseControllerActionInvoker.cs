using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Internal;
using VitalChoice.Validation.Controllers;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

namespace VitalChoice.Validation.Controllers
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
             IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
             ITempDataDictionary tempData,
             ILoggerFactory loggerFactory,
            int maxModelValidationErrors)
            : base(
             actionContext,
             filterProviders,
             controllerFactory,
             descriptor,
             inputFormatters,
             outputFormatters,
             controllerActionArgumentBinder,
             modelBinders,
             modelValidatorProviders,
             valueProviderFactories,
             actionBindingContextAccessor,
             tempData,
             loggerFactory,
             maxModelValidationErrors)
        {

        }

        protected override async Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            var baseController = actionExecutingContext.Controller as BaseController;
            if (baseController != null)
            {
                baseController.Configure();
            }
            return await base.InvokeActionAsync(actionExecutingContext);
        }
    }
}