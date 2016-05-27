using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Core.Base
{
    public abstract class BaseController : Controller
    {
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
                Settings.SetMode(controlMode.ViewModeType, controlMode.Mode, ControllerContext.ActionDescriptor.Name);
            }
        }

        private ControlModeAttribute GetControlMode()
        {
            var controllerAction = ControllerContext.ActionDescriptor;
            var action = controllerAction.MethodInfo;
            return action.GetCustomAttribute<ControlModeAttribute>(false);
        }
    }
}