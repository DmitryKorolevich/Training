using System;
using Microsoft.AspNet.Mvc;

namespace VitalChoice.Validation.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal sealed class GetActionNameAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //context.Controller.Request.Properties["actionName"] = context.ActionDescriptor.Name;
            base.OnActionExecuting(context);
        }
    }
}