﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Business.Services.Contracts.Product;

namespace VC.Public.Controllers.Content
{
    public class BaseContentController : Controller
    {
        protected readonly IContentViewService ContentService;
        protected readonly IProductViewService ProductViewService;
        protected readonly IProductViewService productViewService;

        public BaseContentController(IContentViewService contentService)
        {
            this.ContentService = contentService;
        }

        public BaseContentController(IProductViewService productViewService)
        {
            this.ProductViewService = productViewService;
        }

        //Get params fron action params and all aditional query params
        protected Dictionary<string, object> GetParameters()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var actionParam in ActionContext.ActionDescriptor.Parameters)
            {
                var valueItem = BindingContext.ValueProvider.GetValueAsync(actionParam.Name).Result;
                if (valueItem != null)
                {
                    result.Add(actionParam.Name, valueItem.RawValue);
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