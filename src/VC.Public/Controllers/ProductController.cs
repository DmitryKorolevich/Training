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
using VitalChoice.Business.Services.Contracts.Product;

namespace VitalChoice.Public.Controllers.Content
{
    public class ProductController : BaseContentController
    {
        public ProductController(IProductViewService productViewService) : base(productViewService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            ExecutedContentItem toReturn = await productViewService.GetProductCategoryContentAsync(GetParameters());
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
            ExecutedContentItem toReturn = await productViewService.GetProductCategoryContentAsync(GetParameters(), url);
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