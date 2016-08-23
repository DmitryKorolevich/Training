using System;
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
        private readonly ICategoryService _categoryService;
        private readonly IRecipeService _recipeService;

        public RecipeController(
            IRecipeCategoryViewService recipeCategoryViewService,
            IRecipeViewService recipeViewService,
            ICategoryService categoryService,
            IRecipeService recipeService)
        {
            _recipeCategoryViewService = recipeCategoryViewService;
            _recipeViewService = recipeViewService;
            _categoryService = categoryService;
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _recipeCategoryViewService.GetContentAsync(ControllerContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _recipeCategoryViewService.GetContentAsync(ControllerContext, User);
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
            var toReturn = await _recipeViewService.GetContentAsync(ControllerContext, User);
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
    }
}