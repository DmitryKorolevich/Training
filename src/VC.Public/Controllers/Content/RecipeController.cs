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
using VitalChoice.Data.Repositories;
using Microsoft.Data.Entity;
using VitalChoice.Business.Queries.Content;

namespace VitalChoice.Public.Content.Controllers
{
    public class RecipeController : BaseContentController
    {
        private readonly ILogViewService logViewService;
        private readonly IMasterContentService mast;
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;

        public RecipeController(IContentService contentService, ILogViewService logViewService, IMasterContentService mast,
            IRepositoryAsync<MasterContentItem> masterContentItemRepository) : base(contentService)
        {
            this.logViewService = logViewService;
            this.mast = mast;
            this.masterContentItemRepository = masterContentItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var res = await mast.DeleteMasterContentItemAsync(4);

            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.Recipe, GetParameters());
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
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.Recipe, GetParameters(), url);
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
            ExecutedContentItem toReturn = await contentService.GetContentItemContentAsync(ContentType.Recipe, GetParameters(), url);
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