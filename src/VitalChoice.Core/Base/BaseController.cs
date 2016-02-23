using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using Microsoft.AspNet.Mvc.Routing;
using VitalChoice.Ecommerce.Context;

namespace VitalChoice.Core.Base
{
    public abstract class BaseController: Controller
    {
        protected BaseController()
        {
            Settings = ControllerSettings.Create();
        }

        [NonAction]
        public bool Validate<TViewMode>(BaseModel<TViewMode> model)
            where TViewMode: class, IMode
        {
            model.Mode = (TViewMode)Settings.ValidationMode;
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
            var markedMethods = GetType().GetTypeInfo().DeclaredMethods.Where(m => m.Name == ActionContext.ActionDescriptor.Name).ToList();
            MethodInfo markedMethod = null;
            var httpActionMethod = ActionContext.HttpContext.Request.Method;
            foreach(var method in markedMethods)
            {
                var httpMethodAttributes = method.GetCustomAttributes(typeof(HttpMethodAttribute), false).Select(p => p as HttpMethodAttribute).ToList();
                foreach(var httpMethodAttribute in httpMethodAttributes)
                {
                    foreach(var httpMethod in httpMethodAttribute.HttpMethods)
                    {
                        if(httpMethod== httpActionMethod)
                        {
                            markedMethod = method;
                        }
                    }
                }
            }
            var controlMode = markedMethod?.GetCustomAttributes(typeof(ControlModeAttribute), false).SingleOrDefault() as ControlModeAttribute;
            if (controlMode != null)
            {
                Settings.SetMode(controlMode.ViewModeType, controlMode.Mode, markedMethod.Name);
            }
        }
    }
}