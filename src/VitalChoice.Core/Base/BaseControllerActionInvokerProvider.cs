using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;

namespace VitalChoice.Core.Base
{

    public class ValidationActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IInputFormattersProvider _inputFormatters;
        private readonly IModelBinderProvider _modelBinders;
        private readonly IModelValidatorProviderProvider _modelValidatorProviders;
        private readonly IValueProviderFactoryProvider _valueProviderFactories;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;
        private readonly ITempDataDictionary _tempData;
        private readonly ILoggerFactory _loggerFactory;
        //private readonly int _maxModelValidationErrors;

        public ValidationActionInvokerProvider(
                    IControllerFactory controllerFactory,
                    IEnumerable<IFilterProvider> filterProviders,
                    IControllerActionArgumentBinder argumentBinder,
                    IInputFormattersProvider inputFormattersProvider,
                    IModelBinderProvider modelBinderProvider,
                    IModelValidatorProviderProvider modelValidatorProviderProvider,
                    IValueProviderFactoryProvider valueProviderFactoryProvider,
                    IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
                    ITempDataDictionary tempData,
                    ILoggerFactory loggerFactory)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
            _argumentBinder = argumentBinder;
            _inputFormatters = inputFormattersProvider;//new InputFormattersProvider(optionsAccessor.Options.InputFormatters);
            _modelBinders = modelBinderProvider;
            _modelValidatorProviders = modelValidatorProviderProvider;
            _valueProviderFactories = valueProviderFactoryProvider;
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _tempData = tempData;
            _loggerFactory = loggerFactory;
            //_maxModelValidationErrors = optionsAccessor.Options.MaxModelValidationErrors;
        }

        public int Order => DefaultOrder.DefaultFrameworkSortOrder;

        /// <inheritdoc />
        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new BaseControllerActionInvoker(
                                                    context.ActionContext,
                                                    _filterProviders,
                                                    _controllerFactory,
                                                    actionDescriptor,
                                                    _inputFormatters,
                                                    _argumentBinder,
                                                    _modelBinders,
                                                    _modelValidatorProviders,
                                                    _valueProviderFactories,
                                                    _actionBindingContextAccessor,
                                                    _tempData);
            }
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}