using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VitalChoice.Core.Base
{
    public class CustomControllerActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly ControllerActionInvokerCache _controllerActionInvokerCache;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly int _maxModelValidationErrors;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        public int Order => -1000;

        public CustomControllerActionInvokerProvider(IControllerFactory controllerFactory,
            ControllerActionInvokerCache controllerActionInvokerCache, IControllerActionArgumentBinder argumentBinder,
            IOptions<MvcOptions> optionsAccessor, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource)
        {
            _controllerFactory = controllerFactory;
            _controllerActionInvokerCache = controllerActionInvokerCache;
            _argumentBinder = argumentBinder;
            _inputFormatters = optionsAccessor.Value.InputFormatters.ToArray();
            _modelValidatorProviders = optionsAccessor.Value.ModelValidatorProviders.ToArray();
            _valueProviderFactories = optionsAccessor.Value.ValueProviderFactories.ToArray();
            _maxModelValidationErrors = optionsAccessor.Value.MaxModelValidationErrors;
            _logger = loggerFactory.CreateLogger<ControllerActionInvoker>();
            _diagnosticSource = diagnosticSource;
        }

        /// <inheritdoc />
        public void OnProvidersExecuting(ActionInvokerProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            ControllerActionDescriptor descriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null)
                return;
            context.Result = new CustomControllerActionInvoker(context.ActionContext, _controllerActionInvokerCache, _controllerFactory,
                descriptor, _inputFormatters, _argumentBinder, _modelValidatorProviders, _valueProviderFactories, _logger, _diagnosticSource,
                _maxModelValidationErrors);
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}