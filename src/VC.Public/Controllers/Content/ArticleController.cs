using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class ArticleController : BaseContentController
    {
        private readonly IArticleCategoryViewService _articleCategoryViewService;
        private readonly IArticleViewService _articleViewService;
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;

        public ArticleController(
            IArticleCategoryViewService articleCategoryViewService,
            IArticleViewService articleViewService,
            ICategoryService categoryService,
            IArticleService articleService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _articleCategoryViewService = articleCategoryViewService;
            _articleViewService = articleViewService;
            _categoryService = categoryService;
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _articleCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _articleCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> CategoryByIdOld([FromQuery]string cat)
        {
            int idold;
            if (Int32.TryParse(cat, out idold))
            {
                var item = await _categoryService.GetCategoryByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url) && item?.Type == ContentType.ArticleCategory)
                {
                    return RedirectPermanent($"/articles/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Article(string url)
        {
            var toReturn = await _articleViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> ArticleByIdOld([FromQuery]string id)
        {
            int idold;
            if (Int32.TryParse(id, out idold))
            {
                var item = await _articleService.GetArticleByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url))
                {
                    return RedirectPermanent($"/article/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }
    }
}