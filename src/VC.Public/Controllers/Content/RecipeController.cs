using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class RecipeController : BaseContentController
    {
        private readonly IContentEditService _contentService;

        public RecipeController(IContentEditService contentService)
	    {
		    _contentService = contentService;
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.RecipeCategory, GetParameters());
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
            //var toReturn = await _contentService.GetCategoryContentAsync(ContentType.RecipeCategory, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Recipe(string url)
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetContentItemContentAsync(ContentType.Recipe, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> EditContent(int id) {
            var item = await _contentService.GetContentItemAsync(id);
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
            var item = await _contentService.GetContentItemAsync(id);
            if (item != null)
            {
                item.Template = model.Template;
                item.Updated = DateTime.Now;
                await _contentService.UpdateContentItemAsync(item);
                return RedirectToAction("EditContent", new {id = id});
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> EditMasterContent(int id)
        {
            var item = await _contentService.GetMasterContentItemAsync(id);
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
            var item = await _contentService.GetMasterContentItemAsync(id);
            if (item != null)
            {
                item.Template = model.Template;
                item.Updated = DateTime.Now;
                await _contentService.UpdateMasterContentItemAsync(item);
                return RedirectToAction("EditMasterContent", new { id = id });
            }
            return BaseNotFoundView();
        }
    }
}