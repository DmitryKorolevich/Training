using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using VitalChoice.Public.Models;
using VitalChoice.Business.Services.Contracts.Content;

namespace VitalChoice.Public.Content.Controllers
{
    public class BaseContentController : Controller
    {
        protected readonly IContentViewService contentService;

        public BaseContentController(IContentViewService contentService)
        {
            this.contentService = contentService;
        }

        //Get params fron action params and all aditional query params
        protected Dictionary<string, object> GetParameters()
        {
            Dictionary<string, object> toReturn = new Dictionary<string, object>();
            foreach (var actionParam in ActionContext.ActionDescriptor.Parameters)
            {
                var valueItem = BindingContext.ValueProvider.GetValueAsync(actionParam.Name).Result;
                if (valueItem != null)
                {
                    toReturn.Add(actionParam.Name, valueItem.RawValue);
                }
            }
            foreach (var queryParam in Request.Query)
            {
                if (!toReturn.ContainsKey(queryParam.Key))
                {
                    toReturn.Add(queryParam.Key, queryParam.Value.FirstOrDefault());
                }
            }
            return toReturn;
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