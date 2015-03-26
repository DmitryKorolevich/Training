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

namespace VitalChoice.Validation.Controllers
{
    public class BaseControllerActionInvoker : ControllerActionInvoker
    {
        public BaseControllerActionInvoker(
            ActionContext actionContext,
            IReadOnlyList<IFilterProvider> filterProviders,
            IControllerFactory controllerFactory,
            ControllerActionDescriptor descriptor,
            IInputFormattersProvider inputFormatterProvider,
            IControllerActionArgumentBinder controllerActionArgumentBinder,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidatorProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            ITempDataDictionary tempData) : base(actionContext, filterProviders, controllerFactory, descriptor, inputFormatterProvider,
                controllerActionArgumentBinder, modelBinderProvider, modelValidatorProviderProvider, valueProviderFactoryProvider,
                actionBindingContextAccessor, tempData)
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