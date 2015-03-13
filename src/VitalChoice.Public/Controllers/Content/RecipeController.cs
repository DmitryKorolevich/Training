using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using VitalChoice.Public.Models;

namespace VitalChoice.Public.Content.Controllers
{
    public class RecipeController : BaseContentController
    {
        public RecipeController(IContentService contentService) : base(contentService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Category(int? categoryid=null)
        {
            ExecutedContentItem toReturn = await contentService.GetCategoryContentAsync(ContentType.Recipe, categoryid);
            return BaseView(new ContentPageViewModel(toReturn));
        }

        [HttpGet]
        public async Task<IActionResult> Recipe(int recipeid)
        {
            throw new NotImplementedException();
        }
    }
}