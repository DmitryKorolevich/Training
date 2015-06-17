using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;

namespace VitalChoice.Core.Base
{
    public class BaseControllerActionInvoker : ControllerActionInvoker
    {
        public BaseControllerActionInvoker(
             ActionContext actionContext,
             IReadOnlyList<IFilterProvider> filterProviders,
             IControllerFactory controllerFactory,
             ControllerActionDescriptor descriptor,
             IInputFormattersProvider inputFormatters,
             IControllerActionArgumentBinder controllerActionArgumentBinder,
             IModelBinderProvider modelBinders,
             IModelValidatorProviderProvider modelValidatorProviders,
             IValueProviderFactoryProvider valueProviderFactories,
             IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
             ITempDataDictionary tempData)
            : base(
             actionContext,
             filterProviders,
             controllerFactory,
             descriptor,
             inputFormatters,
             controllerActionArgumentBinder,
             modelBinders,
             modelValidatorProviders,
             valueProviderFactories,
             actionBindingContextAccessor,
             tempData)
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