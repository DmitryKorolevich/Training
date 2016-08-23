using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using VitalChoice.Infrastructure.Domain.Constants;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.Services;

namespace VC.Public.Controllers.Content
{
    public class ContentPageController : BaseContentController
    {
        private readonly IContentPageViewService _contentPageViewService;
        private readonly IContentPageService _contentPageService;

        public ContentPageController(
            IContentPageViewService contentPageViewService,
            IContentPageService contentPageService)
        {
            _contentPageViewService = contentPageViewService;
            _contentPageService = contentPageService;
        }

        [HttpGet]
        public async Task<IActionResult> ContentPage(string url)
        {
            var toReturn = await _contentPageViewService.GetContentAsync(ControllerContext, User);

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
                var item = await _contentPageService.GetContentPageByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url))
                {
                    return RedirectPermanent($"/content/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }
    }
}