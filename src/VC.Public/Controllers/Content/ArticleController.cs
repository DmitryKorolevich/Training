using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
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

        public ArticleController(
            IArticleCategoryViewService articleCategoryViewService,
            IArticleViewService articleViewService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _articleCategoryViewService = articleCategoryViewService;
            _articleViewService = articleViewService;
        }

        private readonly IContentEditService _contentService;

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
        public async Task<IActionResult> Article(string url)
        {
            var toReturn = await _articleViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}