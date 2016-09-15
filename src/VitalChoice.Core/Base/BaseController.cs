using System;
using System.Reflection;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace VitalChoice.Core.Base
{
    public abstract class BaseController : Controller
    {
        [NonAction]
        public bool Validate<TViewMode>(BaseModel<TViewMode> model)
            where TViewMode : class, IMode
        {
            var controlMode = GetControlMode();
            if (controlMode != null &&
                (controlMode.ViewModeType == typeof(TViewMode) || controlMode.ViewModeType.IsSubclassOf(typeof(TViewMode))))
            {
                model.Mode = (TViewMode)CreateMode(controlMode.ViewModeType, controlMode.Mode,
                    ControllerContext.ActionDescriptor.ActionName);
            }
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
            var controlMode = GetControlMode();
            if (controlMode != null)
            {
                model.Mode = CreateMode(controlMode.ViewModeType, controlMode.Mode,
                    ControllerContext.ActionDescriptor.ActionName);
            }
            model.Validate();
            if (model.IsValid)
            {
                return ModelState.IsValid;
            }
            ModelState.MaxAllowedErrors = 1000;
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

        private ControlModeAttribute GetControlMode()
        {
            var controllerAction = ControllerContext.ActionDescriptor;
            var action = controllerAction.MethodInfo;
            return action.GetCustomAttribute<ControlModeAttribute>(false);
        }

        public static IMode CreateMode(Type viewModeType, object mode, string action)
        {
            if (!typeof(IMode).IsAssignableFrom(viewModeType))
                throw new ArgumentException("viewModeType should implement IMode");

            var viewMode = (IMode)Activator.CreateInstance(viewModeType);
            viewMode.Mode = mode;
            return viewMode;
        }
    }
}