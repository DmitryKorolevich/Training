using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;

namespace QRProject.Api.Controllers.Base
{
    public class BaseControllerActionInvoker : ControllerActionInvoker
    {
        public BaseControllerActionInvoker(ActionContext actionContext,
            INestedProviderManager<FilterProviderContext> filterProvider, IControllerFactory controllerFactory,
            ControllerActionDescriptor descriptor, IInputFormattersProvider inputFormatterProvider,
            IControllerActionArgumentBinder controllerActionArgumentBinder, IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidatorProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor)
            : base(
                actionContext, filterProvider, controllerFactory, descriptor, inputFormatterProvider,
                controllerActionArgumentBinder, modelBinderProvider, modelValidatorProviderProvider,
                valueProviderFactoryProvider, actionBindingContextAccessor)
        {
        }

        protected override Task<IActionResult> InvokeActionAsync(ActionExecutingContext actionExecutingContext)
        {
            var baseController = actionExecutingContext.Controller as BaseController;
            if (baseController != null)
            {
                baseController.Configure(actionExecutingContext.ActionDescriptor.Name);
            }
            return base.InvokeActionAsync(actionExecutingContext);
        }
    }
}