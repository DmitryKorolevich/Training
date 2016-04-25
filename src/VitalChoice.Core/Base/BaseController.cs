using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using Microsoft.AspNet.Mvc.Routing;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Core.Base
{
    public abstract class BaseController : Controller
    {
        private static volatile Dictionary<ControlModeSearchInfo, ControlModeAttribute> _cachedControllers =
            new Dictionary<ControlModeSearchInfo, ControlModeAttribute>();

        protected BaseController()
        {
            Settings = ControllerSettings.Create();
        }

        [NonAction]
        public bool Validate<TViewMode>(BaseModel<TViewMode> model)
            where TViewMode : class, IMode
        {
            model.Mode = (TViewMode) Settings.ValidationMode;
            model.Validate();
            if (model.IsValid)
            {
                return ModelState.IsValid;
            }
            foreach (var validationError in model.Errors)
            {
                ModelState.AddModelError(validationError.Key, validationError.Value);
            }
            return false;
        }

        [NonAction]
        public bool Validate<TViewMode>(BaseModel<TViewMode> model, TViewMode mode)
            where TViewMode : class, IMode
        {
            model.Mode = mode;
            model.Validate();
            if (model.IsValid)
            {
                return ModelState.IsValid;
            }
            foreach (var validationError in model.Errors)
            {
                ModelState.AddModelError(validationError.Key, validationError.Value);
            }
            return false;
        }

        [NonAction]
        public bool Validate(BaseModel model)
        {
            model.Mode = Settings.ValidationMode;
            model.Validate();
            if (model.IsValid)
            {
                return ModelState.IsValid;
            }
            foreach (var validationError in model.Errors)
            {
                ModelState.AddModelError(validationError.Key, validationError.Value);
            }
            return false;
        }

        [NonAction]
        public bool Validate(BaseModel model, IMode mode)
        {
            model.Mode = mode;
            model.Validate();
            if (model.IsValid)
            {
                return ModelState.IsValid;
            }
            foreach (var validationError in model.Errors)
            {
                ModelState.AddModelError(validationError.Key, validationError.Value);
            }
            return false;
        }

        [HttpGet]
        public Result<ControllerSettings> GetSettings()
        {
            return Settings;
        }

        public ControllerSettings Settings { get; }

        [NonAction]
        public virtual void Configure()
        {
            var controlMode = GetControlMode();
            if (controlMode != null)
            {
                Settings.SetMode(controlMode.ViewModeType, controlMode.Mode, ActionContext.ActionDescriptor.Name);
            }
        }

        private struct ControlModeSearchInfo : IEquatable<ControlModeSearchInfo>
        {
            public bool Equals(ControlModeSearchInfo other)
            {
                return string.Equals(_actionName, other._actionName) && _controllerType == other._controllerType &&
                       string.Equals(_httpMethod, other._httpMethod);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ControlModeSearchInfo && Equals((ControlModeSearchInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _actionName.GetHashCode();
                    hashCode = (hashCode*397) ^ _controllerType.GetHashCode();
                    hashCode = (hashCode*397) ^ _httpMethod.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(ControlModeSearchInfo left, ControlModeSearchInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ControlModeSearchInfo left, ControlModeSearchInfo right)
            {
                return !left.Equals(right);
            }

            private readonly Type _controllerType;
            private readonly string _actionName;
            private readonly string _httpMethod;

            public ControlModeSearchInfo(Type controllerType, string actionName, string httpMethod)
            {
                _controllerType = controllerType;
                _actionName = actionName;
                _httpMethod = httpMethod;
            }
        }

        private ControlModeAttribute GetControlMode()
        {
            ControlModeAttribute result;
            var controllerAction = ActionContext.ActionDescriptor as ControllerActionDescriptor;
            if (controllerAction != null)
            {
                var action = controllerAction.MethodInfo;
                return action.GetCustomAttribute<ControlModeAttribute>(false);
            }
            var controllerType = GetType();
            var actionName = ActionContext.ActionDescriptor.Name;
            var httpMethod = ActionContext.HttpContext.Request.Method;
            var searchKey = new ControlModeSearchInfo(controllerType, actionName, httpMethod);
            if (_cachedControllers.TryGetValue(new ControlModeSearchInfo(controllerType, actionName, httpMethod), out result))
            {
                return result;
            }
            lock (_cachedControllers)
            {
                var actionMethods = GetType().GetTypeInfo().DeclaredMethods.Where(m => m.Name == actionName);
                var action = actionMethods.FirstOrDefault(
                    m => m.GetCustomAttributes<HttpMethodAttribute>(false).Any(a => a.HttpMethods.Any(hm => hm == httpMethod)));
                result = action?.GetCustomAttribute<ControlModeAttribute>(false);
                var newDict = new Dictionary<ControlModeSearchInfo, ControlModeAttribute>(_cachedControllers) {{searchKey, result}};
                _cachedControllers = newDict;
            }
            return result;
        }
    }
}