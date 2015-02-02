using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Controllers
{
    public abstract class BaseController: Controller
    {
        protected BaseController()
        {
            Settings = ControllerSettings.Create();
            Parse(GetType(), Settings);
        }

        [NonAction]
        public T ConvertWithValidate<T, TViewMode>(Model<T, TViewMode> model, T dbEntity = null)
            where T: class, new()
            where TViewMode: class, IMode
        {
            string actionName = ActionContext.ActionDescriptor.Name;
            //string actionName = ControllerContext.Request.Properties["actionName"] as string;

            model.Mode = Settings.GetValidationMode<TViewMode>(actionName);
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
        
        [NonAction]
        internal abstract void Configure(string actionName);

        public ControllerSettings Settings { get; private set; }

        [NonAction]
        protected void Parse(Type controllerType, ControllerSettings settings)
        {
            var markedMethods = controllerType.GetTypeInfo().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ControlModeAttribute), false).Any());
            foreach (var markedMethod in markedMethods) {
                var controlMode =
                    markedMethod.GetCustomAttributes(typeof(ControlModeAttribute), false).SingleOrDefault() as
                    ControlModeAttribute;
                if (controlMode != null) {
                    settings.SetMode(controlMode.ViewModeType, controlMode.Mode, markedMethod.Name);
                }
            }
        }
    }
}