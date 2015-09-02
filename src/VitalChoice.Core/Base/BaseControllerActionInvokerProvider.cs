using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Notification;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Core.Base
{

    public class ValidationActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IReadOnlyList<IFilterProvider> _filterProviders;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IActionBindingContextAccessor _actionBindingContextAccessor;
        private readonly ILoggerProviderExtended _loggerProvider;
        private readonly INotifier _notifier;
        //private readonly int _maxModelValidationErrors;

        public ValidationActionInvokerProvider(
                    IControllerFactory controllerFactory,
                    IReadOnlyList<IFilterProvider> filterProviders,
                    IControllerActionArgumentBinder argumentBinder,
                    IActionBindingContextAccessor actionBindingContextAccessor,
                    ILoggerProviderExtended loggerProvider, IOptions<MvcOptions> mvcOptions, INotifier notifier)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = new ReadOnlyCollection<IFilterProvider>(filterProviders.OrderBy(item => item.Order).ToList());
            _argumentBinder = argumentBinder;
            _outputFormatters = new ReadOnlyCollection<IOutputFormatter>(mvcOptions.Options.OutputFormatters);
            _inputFormatters = new ReadOnlyCollection<IInputFormatter>(mvcOptions.Options.InputFormatters);
            _modelBinders = new ReadOnlyCollection<IModelBinder>(mvcOptions.Options.ModelBinders);
            _modelValidatorProviders = new ReadOnlyCollection<IModelValidatorProvider>(mvcOptions.Options.ModelValidatorProviders);
            _valueProviderFactories = new ReadOnlyCollection<IValueProviderFactory>(mvcOptions.Options.ValueProviderFactories);
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _loggerProvider = loggerProvider;
            _notifier = notifier;
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
                                                    _outputFormatters,
                                                    _argumentBinder,
                                                    _modelBinders,
                                                    _modelValidatorProviders,
                                                    _valueProviderFactories,
                                                    _actionBindingContextAccessor,
                                                    _loggerProvider.CreateLoggerDefault(), _notifier);
            }
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}