using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Framework.Internal;
using VitalChoice.Validation.Controllers;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;

namespace VitalChoice.Validation.Controllers
{
    public class ControllerActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly IFilterProvider[] _filterProviders;
        private readonly IReadOnlyList<IInputFormatter> _inputFormatters;
        private readonly IReadOnlyList<IModelBinder> _modelBinders;
        private readonly IReadOnlyList<IModelValidatorProvider> _modelValidatorProviders;
        private readonly IReadOnlyList<IValueProviderFactory> _valueProviderFactories;
        private readonly IScopedInstance<ActionBindingContext> _actionBindingContextAccessor;
        private readonly ITempDataDictionary _tempData;

        public ControllerActionInvokerProvider(
            IControllerFactory controllerFactory,
            IEnumerable<IFilterProvider> filterProviders,
            IControllerActionArgumentBinder argumentBinder,
            IOptions<MvcOptions> optionsAccessor,
            IScopedInstance<ActionBindingContext> actionBindingContextAccessor,
            ITempDataDictionary tempData)
        {
            _controllerFactory = controllerFactory;
            _filterProviders = filterProviders.OrderBy(item => item.Order).ToArray();
            _argumentBinder = argumentBinder;
            _inputFormatters = optionsAccessor.Options.InputFormatters.ToArray();
            _modelBinders = optionsAccessor.Options.ModelBinders.ToArray();
            _modelValidatorProviders = optionsAccessor.Options.ModelValidatorProviders.ToArray();
            _valueProviderFactories = optionsAccessor.Options.ValueProviderFactories.ToArray();
            _actionBindingContextAccessor = actionBindingContextAccessor;
            _tempData = tempData;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder; }
        }

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