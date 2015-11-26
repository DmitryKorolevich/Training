using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using VC.Public.Controllers.Content;
using VC.Public.Models;
using VC.Public.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Controllers
{
    public class ProductController : BaseContentController
    {
	    private readonly ICategoryViewService _categoryViewService;
	    private readonly IProductViewService _productViewService;
	    private readonly ReCaptchaValidator _reCaptchaValidator;
	    private readonly IProductService _productService;
	    private readonly IProductReviewService _productReviewService;

	    public ProductController(ICategoryViewService categoryViewService, IProductViewService productViewService, ReCaptchaValidator reCaptchaValidator, IProductService productService, IProductReviewService productReviewService)
	    {
		    _categoryViewService = categoryViewService;
		    _productViewService = productViewService;
		    _reCaptchaValidator = reCaptchaValidator;
		    _productService = productService;
		    _productReviewService = productReviewService;
	    }

	    private async Task PopulateProductReviewAssets(Guid id)
	    {
			var productTransfer = await _productService.SelectTransferAsync(id, true);

			ViewBag.Image = productTransfer.ProductDynamic.Data.Thumbnail;
			ViewBag.Name = productTransfer.ProductDynamic.Name;
			ViewBag.SubTitle = productTransfer.ProductDynamic.Data.SubTitle;
		}

	    private IList<CustomerTypeCode> GetCategoryMenuAvailability()
	    {
		    return User.Identity.IsAuthenticated
			    ? (User.IsInRole(IdentityConstants.WholesaleCustomer)
				    ? new List<CustomerTypeCode>() {CustomerTypeCode.Wholesale, CustomerTypeCode.All}
				    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All})
			    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All};
			    //todo: refactor when authentication mechanism gets ready
	    }

	    [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _categoryViewService.GetProductCategoryContentAsync(GetCategoryMenuAvailability(), GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _categoryViewService.GetProductCategoryContentAsync(GetCategoryMenuAvailability(), GetParameters());
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

		[HttpGet]
		public async Task<IActionResult> Product(string url)
		{
			var toReturn = await _productViewService.GetProductPageContentAsync(GetCategoryMenuAvailability(), GetParameters());
			if (toReturn != null)
			{
				return View("~/Views/Content/ProductPage.cshtml", new ContentPageViewModel(toReturn));
			}
			return BaseNotFoundView();
		}

		[HttpGet]
		public async Task<IActionResult> AddReview(Guid id)
		{
			await PopulateProductReviewAssets(id);

			return PartialView("_AddReview", new AddReviewModel {ProductId = id});
		}

		[HttpPost]
		public async Task<IActionResult> AddReview(AddReviewModel model)
		{
			if (Validate(model))
			{
				if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
				{
					ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
				}
				else
				{
					await _productReviewService.UpdateProductReviewAsync(new ProductReview()
					{
						IdProduct = await _productService.GetProductInternalIdAsync(model.ProductId),
						StatusCode = RecordStatusCode.NotActive,
						CustomerName = model.CustomerName?.Trim(),
						Email = model.Email?.Trim(),
						Title = model.ReviewTitle?.Trim(),
						Rating = model.Rating,
						Description = model.Review
					});

					ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];
					return PartialView("_AddReviewInner", model);
				}
			}

			return PartialView("_AddReviewInner", model);
		}
	}
}