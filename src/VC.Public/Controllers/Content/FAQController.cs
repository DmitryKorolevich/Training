﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class FaqController : BaseContentController
    {
        private readonly IContentEditService _contentService;

        public FaqController(IContentEditService contentService)
	    {
		   _contentService = contentService;
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.FaqCategory, GetParameters());
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
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.FaqCategory, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> FAQ(string url)
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetContentItemContentAsync(ContentType.Faq, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }
    }
}