using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Mvc;

namespace VitalChoice.Validation.Controllers
{
    public class BaseControllerActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly INestedProviderManager<FilterProviderContext> _filterProvider;
        private readonly IInputFormattersProvider _inputFormattersProvider;
        private readonly IModelBinderProvider _modelBinderProvider;
        private readonly IModelValidatorProviderProvider _modelValidationProviderProvider;
        private readonly IValueProviderFactoryProvider _valueProviderFactoryProvider;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;

        public BaseControllerActionInvokerProvider(
            IControllerFactory controllerFactory,
            IInputFormattersProvider inputFormattersProvider,
            INestedProviderManager<FilterProviderContext> filterProvider,
            IControllerActionArgumentBinder argumentBinder,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidationProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor)
        {
            _controllerFactory = controllerFactory;
            _inputFormattersProvider = inputFormattersProvider;
            _filterProvider = filterProvider;
            _argumentBinder = argumentBinder;
            _modelBinderProvider = modelBinderProvider;
            _modelValidationProviderProvider = modelValidationProviderProvider;
            _valueProviderFactoryProvider = valueProviderFactoryProvider;
            _actionBindingContextAccessor = actionBindingContextAccessor;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder; }
        }

        public void Invoke(ActionInvokerProviderContext context, Action callNext)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new BaseControllerActionInvoker(
                                    context.ActionContext,
                                    _filterProvider,
                                    _controllerFactory,
                                    actionDescriptor,
                                    _inputFormattersProvider,
                                    _argumentBinder,
                                    _modelBinderProvider,
                                    _modelValidationProviderProvider,
                                    _valueProviderFactoryProvider,
                                    _actionBindingContextAccessor);
            }

            callNext();
        }
    }
}