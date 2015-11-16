using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class FaqController : BaseContentController
    {
        public FaqController(IContentEditService contentService) : base(contentService)
        {
        }

	    public FaqController(IContentViewService contentService)
	    {
		   _contentService = contentService;
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await ContentService.GetCategoryContentAsync(ContentType.FaqCategory, GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await ContentService.GetCategoryContentAsync(ContentType.FaqCategory, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> FAQ(string url)
        {
            var toReturn = await ContentService.GetContentItemContentAsync(ContentType.Faq, GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}