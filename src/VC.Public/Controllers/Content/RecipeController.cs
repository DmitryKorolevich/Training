using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.WebEncoders;
using Templates.Strings.Web;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Public.Content.Controllers;
using VitalChoice.Public.Models;
using VitalChoice.Data.Repositories;
using Microsoft.Data.Entity;
using VitalChoice.Business.Queries.Content;

namespace VitalChoice.Public.Controllers.Content
{
    public class RecipeController : BaseContentController
    {
        private readonly ILogViewService _logViewService;
        private readonly IMasterContentService mast;
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;

        public RecipeController(IContentService contentService, ILogViewService logViewService, IMasterContentService mast,
            IRepositoryAsync<MasterContentItem> masterContentItemRepository) : base(contentService)
        {
            this._logViewService = logViewService;
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

        [HttpGet]
        public async Task<IActionResult> EditContent(int id) {
            var item = await contentService.GetContentItemAsync(id);
            if (item != null)
            {
                return View("~/Views/Content/EditDemo.cshtml", new ContentEditModel
                {
                    Id = item.Id,
                    Template = item.Template
                });
            }
            return BaseNotFoundView();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContent(int id, ContentEditModel model)
        {
            var item = await contentService.GetContentItemAsync(id);
            if (item != null)
            {
                item.Template = model.Template;
                item.Updated = DateTime.Now;
                await contentService.UpdateContentItemAsync(item);
                return RedirectToAction("EditContent", new {id = id});
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> EditMasterContent(int id)
        {
            var item = await contentService.GetMasterContentItemAsync(id);
            if (item != null) {
                return View("~/Views/Content/EditDemo.cshtml", new ContentEditModel
                {
                    Id = item.Id,
                    Template = item.Template,
                    Master = true
                });
            }
            return BaseNotFoundView();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMasterContent(int id, ContentEditModel model) {
            var item = await contentService.GetMasterContentItemAsync(id);
            if (item != null)
            {
                item.Template = model.Template;
                item.Updated = DateTime.Now;
                await contentService.UpdateMasterContentItemAsync(item);
                return RedirectToAction("EditMasterContent", new { id = id });
            }
            return BaseNotFoundView();
        }
    }
}