﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using VitalChoice.Public.Models;

namespace VitalChoice.Public.Content.Controllers
{
    public class RecipeController : BaseContentController
    {
        public RecipeController(IContentService contentService) : base(contentService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.Recipe);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            else
            {
                return BaseNotFoundView();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.Recipe, url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            else
            {
                return BaseNotFoundView();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Recipe(string url)
        {
            ExecutedContentItem toReturn = await contentService.GetContentItemContentAsync(ContentType.Recipe, url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            else
            {
                return BaseNotFoundView();
            }
        }
    }
}