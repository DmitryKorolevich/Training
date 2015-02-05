using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VitalChoice.Validation.Controllers
{
    public abstract class BaseController: Controller
    {
        protected BaseController()
        {
            Settings = ControllerSettings.Create();
        }

        [NonAction]
        public T ConvertWithValidate<T, TViewMode>(Model<T, TViewMode> model, T dbEntity = null)
            where T: class, new()
            where TViewMode: class, IMode
        {
            model.Mode = (TViewMode)Settings.ValidationMode;
            model.Validate();
            if (!model.IsValid) {
                foreach (var validationError in model.Errors) {
                    ModelState.AddModelError(validationError.Key, validationError.Value);
                }
            }
            else {
                return dbEntity == null ? model.Convert() : model.Update(dbEntity);
            }
            return null;
        }

        [HttpGet]
        public Result<ControllerSettings> GetSettings()
        {
            return Settings;
        }
        
        public ControllerSettings Settings { get; private set; }

        [NonAction]
        public virtual void Configure()
        {
            var markedMethod = this.GetType().GetTypeInfo().DeclaredMethods.Where(m => m.Name == ActionContext.ActionDescriptor.Name).SingleOrDefault();
            if (markedMethod != null)
            {
                var controlMode = markedMethod.GetCustomAttributes(typeof(ControlModeAttribute), false).SingleOrDefault() as
                    ControlModeAttribute;
                if (controlMode != null)
                {
                    Settings.SetMode(controlMode.ViewModeType, controlMode.Mode, markedMethod.Name);
                }
            }
        }
    }
}