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
using VitalChoice.Business.Services.Contracts.Content;

namespace VitalChoice.Public.Controllers.Content
{
    public class ContentPageController : BaseContentController
    {
        public ContentPageController(IContentViewService contentService) : base(contentService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters());
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
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.ContentPageCategory, GetParameters(), url);
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
        public async Task<IActionResult> ContentPage(string url)
        {
            ExecutedContentItem toReturn = await contentService.GetContentItemContentAsync(ContentType.ContentPage, GetParameters(), url);
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