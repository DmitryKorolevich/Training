using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace VitalChoice.Validation.Base
{
    //public class InputFormattersProvider : IInputFormattersProvider
    //{
    //    public InputFormattersProvider(IEnumerable<InputFormatterDescriptor> inputFormatters)
    //    {
    //        InputFormatters = inputFormatters.Select(d => d.Instance).ToArray();
    //    }

    //    public IReadOnlyList<IInputFormatter> InputFormatters
    //    {
    //        get;
    //    }
    //}

    public class ValidationActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IOutputFormatter> _outputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;
        private readonly ITempDataDictionary _tempData;
        private readonly ILoggerFactory _loggerFactory;
        //private readonly int _maxModelValidationErrors;

        public ValidationActionInvokerProvider(
                    IControllerFactory controllerFactory,
                    IEnumerable<IFilterProvider> filterProviders,
                    IControllerActionArgumentBinder argumentBinder,
                    IOptions<MvcOptions> options,
                    IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
                    ITempDataDictionary tempData,
                    ILoggerFactory loggerFactory)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
            _argumentBinder = argumentBinder;
            _inputFormatters = new ReadOnlyCollection<IInputFormatter>(options.Options.InputFormatters);//new InputFormattersProvider(optionsAccessor.Options.InputFormatters);
            _modelBinders = new ReadOnlyCollection<IModelBinder>(options.Options.ModelBinders);
            _modelValidatorProviders = new ReadOnlyCollection<IModelValidatorProvider>(options.Options.ModelValidatorProviders);
            _valueProviderFactories = new ReadOnlyCollection<IValueProviderFactory>(options.Options.ValueProviderFactories);
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _tempData = tempData;
            _loggerFactory = loggerFactory;
            _outputFormatters = new ReadOnlyCollection<IOutputFormatter>(options.Options.OutputFormatters);
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
                                                    _tempData, _loggerFactory);
            }
        }

        /// <inheritdoc />
        public void OnProvidersExecuted(ActionInvokerProviderContext context)
        {
        }
    }
}