using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Domain.Entities.Content;

namespace VC.Public.Controllers.Content
{
    public class ArticleController : BaseContentController
    {
        public ArticleController(IContentViewService contentService) : base(contentService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            ExecutedContentItem toReturn = await ContentService.GetCategoryContentAsync(ContentType.ArticleCategory, GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            ExecutedContentItem toReturn = await ContentService.GetCategoryContentAsync(ContentType.ArticleCategory, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Article(string url)
        {
            ExecutedContentItem toReturn = await ContentService.GetContentItemContentAsync(ContentType.Article, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}