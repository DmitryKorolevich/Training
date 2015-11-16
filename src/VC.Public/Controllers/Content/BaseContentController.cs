using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Controllers.Content
{
    public class BaseContentController : Controller
    {
        //Get params fron action params and all aditional query params
        protected Dictionary<string, object> GetParameters()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var actionParam in ActionContext.ActionDescriptor.Parameters)
            {
                var values = BindingContext.ValueProvider.GetValue(actionParam.Name).Values;
                foreach (var stringValue in values)
                {
                    result.Add(actionParam.Name, stringValue);
                }
            }
            foreach (var queryParam in Request.Query)
            {
                if (!result.ContainsKey(queryParam.Key))
                {
                    result.Add(queryParam.Key, queryParam.Value.FirstOrDefault());
                }
            }
            return result;
        }

        public virtual ViewResult BaseView(ContentPageViewModel model)
        {
            return View("~/Views/Content/ContentPage.cshtml", model);
        }

        public virtual ViewResult BaseNotFoundView()
        {
            var result = View("~/Views/Shared/Error404.cshtml");
            result.StatusCode = 404;
            return result;
        }
    }
}