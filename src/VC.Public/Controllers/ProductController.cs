using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Controllers.Content;
using VC.Public.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Controllers
{
    public class ProductController : BaseContentController
    {
        public ProductController(IProductViewService productViewService) : base(productViewService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            ExecutedContentItem toReturn = await ProductViewService.GetProductCategoryContentAsync(GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            ExecutedContentItem toReturn = await ProductViewService.GetProductCategoryContentAsync(GetParameters(), url);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}