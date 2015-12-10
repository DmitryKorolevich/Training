using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class ArticleController : BaseContentController
    {
        private readonly IArticleCategoryViewService _articleCategoryViewService;

        public ArticleController(IArticleCategoryViewService articleCategoryViewService)
	    {
            _articleCategoryViewService = articleCategoryViewService;
	    }

        private readonly IContentEditService _contentService;

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _articleCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _articleCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Article(string url)
        {
            throw new NotImplementedException();
            //var toReturn = await _contentService.GetContentItemContentAsync(ContentType.Article, GetParameters(), url);
            //if (toReturn != null)
            //{
            //    return BaseView(new ContentPageViewModel(toReturn));
            //}
            //return BaseNotFoundView();
        }
    }
}