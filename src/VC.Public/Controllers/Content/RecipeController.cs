using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using VitalChoice.Public.Models;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Public.Content.Controllers
{
    public class RecipeController : BaseContentController
    {
        private readonly ILogViewService logViewService;

        public RecipeController(IContentService contentService, ILogViewService logViewService) : base(contentService)
        {
            this.logViewService = logViewService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var logger = LoggerService.GetDefault();
            logger.LogError("test");

            //var pagedList = logViewService.GetCommonItems();
            //var pagedList2 = logViewService.GetCommonItems(logLevel: "fatal");

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