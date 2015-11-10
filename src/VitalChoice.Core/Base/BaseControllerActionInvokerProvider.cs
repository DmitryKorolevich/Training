using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Models.Interfaces;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.AspNet.Mvc.Infrastructure;
using System.Diagnostics.Tracing;
using System;
using System.Diagnostics;

namespace VitalChoice.Core.Base
{
    public class ValidationActionInvokerProvider : IActionInvokerProvider
    {
#pragma warning disable 0618
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IActionBindingContextAccessor _actionBindingContextAccessor;
        private readonly int _maxModelValidationErrors;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _telemetry;

        public ValidationActionInvokerProvider(
            IControllerFactory controllerFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IControllerActionArgumentBinder argumentBinder,
            IOptions<MvcOptions> optionsAccessor,
            IActionBindingContextAccessor actionBindingContextAccessor,
            ILoggerFactory loggerFactory,
            DiagnosticSource telemetry)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
            _argumentBinder = argumentBinder;
            _inputFormatters = optionsAccessor.Value.InputFormatters.ToArray();
            _outputFormatters = optionsAccessor.Value.OutputFormatters.ToArray();
            _modelBinders = optionsAccessor.Value.ModelBinders.ToArray();
            _modelValidatorProviders = optionsAccessor.Value.ModelValidatorProviders.ToArray();
            _valueProviderFactories = optionsAccessor.Value.ValueProviderFactories.ToArray();
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _maxModelValidationErrors = optionsAccessor.Value.MaxModelValidationErrors;
            _logger = loggerFactory.CreateLogger<ControllerActionInvoker>();
            _telemetry = telemetry;
        }
#pragma warning restore 0618

        public int Order
        {
            get { return -1000; }
        }

        /// <inheritdoc />
        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
                    _logger,
                    _telemetry,
                    _maxModelValidationErrors);
            }
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}