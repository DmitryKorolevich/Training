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
using VitalChoice.Infrastructure.Domain.Constants;
using System.Net;
using VitalChoice.Core.Services;

namespace VC.Public.Controllers.Content
{
    public class ContentPageController : BaseContentController
    {
        private readonly IContentEditService _contentService;
        private readonly IContentPageViewService _contentPageViewService;
        private readonly IContentPageService _contentPageService;

        public ContentPageController(
            IContentEditService contentService,
            IContentPageViewService contentPageViewService,
            IContentPageService contentPageService,
            IPageResultService pageResultService) : base(pageResultService)
        {
		    _contentService = contentService;
            _contentPageViewService = contentPageViewService;
            _contentPageService = contentPageService;
        }

        [HttpGet]
        public async Task<IActionResult> ContentPage(string url)
        {
            var toReturn = await _contentPageViewService.GetContentAsync(ActionContext, BindingContext, User);

			switch (url)
			{
				case ContentConstants.NOT_FOUND_PAGE_URL:
					Response.StatusCode = (int)HttpStatusCode.NotFound;
					break;
				case ContentConstants.ACESS_DENIED_PAGE_URL:
					Response.StatusCode = (int)HttpStatusCode.Forbidden;
					break;
			}

			if (toReturn?.Body != null)
            {
	            return BaseView(new ContentPageViewModel(toReturn));
            }
            else
            {
				switch (url)
				{
					case ContentConstants.NOT_FOUND_PAGE_URL:
						return View("NotFound");
					case ContentConstants.ACESS_DENIED_PAGE_URL:
						Response.StatusCode = (int)HttpStatusCode.Forbidden;
						return View("AccessDenied");
				}
			}
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> ContentPageByIdOld([FromQuery]string idpage)
        {
            int idold;
            if (Int32.TryParse(idpage, out idold))
            {
                var item = await _contentPageService.GetContentPageAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url))
                {
                    return RedirectPermanent($"/content/{item.Url}");
                }
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