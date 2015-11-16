﻿using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class ContentPageController : BaseContentController
    {
        public ContentPageController(IContentEditService contentService) : base(contentService)
        {
        }

	    public ContentPageController(IContentViewService contentService)
	    {
		    _contentService = contentService;
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> ContentPage(string url)
        {
            var toReturn = await _contentService.GetContentItemContentAsync(ContentType.ContentPage, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}