﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class RecipeController : BaseContentController
    {
        private readonly IRecipeCategoryViewService _recipeCategoryViewService;
        private readonly IRecipeViewService _recipeViewService;
        //private readonly IContentEditService _contentService;
        private readonly ICategoryService _categoryService;
        private readonly IRecipeService _recipeService;

        public RecipeController(
            IRecipeCategoryViewService recipeCategoryViewService,
            IRecipeViewService recipeViewService,
            //IContentEditService contentService,
            ICategoryService categoryService,
            IRecipeService recipeService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _recipeCategoryViewService = recipeCategoryViewService;
            _recipeViewService = recipeViewService;
            //_contentService = contentService;
            _categoryService = categoryService;
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _recipeCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _recipeCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> CategoryByIdOld([FromQuery]string idCategory)
        {
            int idold;
            if (Int32.TryParse(idCategory, out idold))
            {
                var item = await _categoryService.GetCategoryByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url) && item?.Type==ContentType.RecipeCategory)
                {
                    return RedirectPermanent($"/recipes/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Recipe(string url)
        {
            var toReturn = await _recipeViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> RecipeByIdOld([FromQuery]string RecipeId)
        {
            int idold;
            if (Int32.TryParse(RecipeId, out idold))
            {
                var item = await _recipeService.GetRecipeByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url))
                {
                    return RedirectPermanent($"/recipe/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }

        //[HttpGet]
        //public async Task<IActionResult> EditContent(int id) {
        //    var item = await _contentService.GetContentItemAsync(id);
        //    if (item != null)
        //    {
        //        return View("~/Views/Content/EditDemo.cshtml", new ContentEditModel
        //        {
        //            Id = item.Id,
        //            Template = item.Template
        //        });
        //    }
        //    return BaseNotFoundView();
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateContent(int id, ContentEditModel model)
        //{
        //    var item = await _contentService.GetContentItemAsync(id);
        //    if (item != null)
        //    {
        //        item.Template = model.Template;
        //        item.Updated = DateTime.Now;
        //        await _contentService.UpdateContentItemAsync(item);
        //        return RedirectToAction("EditContent", new {id = id});
        //    }
        //    return BaseNotFoundView();
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditMasterContent(int id)
        //{
        //    var item = await _contentService.GetMasterContentItemAsync(id);
        //    if (item != null) {
        //        return View("~/Views/Content/EditDemo.cshtml", new ContentEditModel
        //        {
        //            Id = item.Id,
        //            Template = item.Template,
        //            Master = true
        //        });
        //    }
        //    return BaseNotFoundView();
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateMasterContent(int id, ContentEditModel model) {
        //    var item = await _contentService.GetMasterContentItemAsync(id);
        //    if (item != null)
        //    {
        //        item.Template = model.Template;
        //        item.Updated = DateTime.Now;
        //        await _contentService.UpdateMasterContentItemAsync(item);
        //        return RedirectToAction("EditMasterContent", new { id = id });
        //    }
        //    return BaseNotFoundView();
        //}
    }
}