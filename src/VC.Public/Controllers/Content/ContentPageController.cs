using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Mvc.Abstractions;
using System.IO;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.Rendering;

namespace VC.Public.Controllers.Content
{
    public class ContentPageController : BaseContentController
    {
        private readonly IContentEditService _contentService;
        private readonly IContentPageViewService _contentPageViewService;

        public ContentPageController(
            IContentEditService contentService,
            IContentPageViewService contentPageViewService)
	    {
		    _contentService = contentService;
            _contentPageViewService = contentPageViewService;
        }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters());
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> ContentPage(string url)
        {
            var toReturn = await _contentPageViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        public async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            ModelStateDictionary modelState = new ModelStateDictionary();
            ViewDataDictionary viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), modelState);
            ITempDataDictionary tempData = HttpContext.RequestServices.GetService(typeof(ITempDataDictionary)) as ITempDataDictionary;
            RouteData routeData = new RouteData();
            ActionDescriptor actionDescriptor = new ActionDescriptor();
            ActionContext actionContext = new ActionContext(HttpContext, routeData, actionDescriptor);

            viewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                var engine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = engine.FindPartialView(actionContext, viewName);

                ViewContext viewContext = new ViewContext(actionContext, viewResult.View, viewData, tempData, sw, new HtmlHelperOptions());
                

                await viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}