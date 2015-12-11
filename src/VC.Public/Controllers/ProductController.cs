using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Net.Http.Server;
using VC.Public.Controllers.Content;
using VC.Public.Models;
using VC.Public.Models.Product;
using VC.Public.Models.Profile;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Products;
using System.Linq;

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

	    private async Task<PagedListEx<ReviewListItemModel>> GetReviewsModel(ProductReviewFilter filter)
	    {
			filter.Sorting.SortOrder = SortOrder.Desc;
		    filter.Sorting.Path = ProductReviewSortPath.DateCreated;

			var productReviews = await _productReviewService.GetProductReviewsAsync(filter);

			var reviewsModel = new PagedListEx<ReviewListItemModel>
			{
				Items = productReviews.Items.Select(p => new ReviewListItemModel()
				{
					DateCreated = p.DateCreated,
					Title = p.Title,
					Review = p.Description,
					CustomerName = p.CustomerName,
					Rating = p.Rating,
				}).ToList(),
				Count = productReviews.Count,
				Index = filter.Paging.PageIndex
			};

		    return reviewsModel;
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
            var toReturn = await _categoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _categoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

		[HttpGet]
		public async Task<IActionResult> Product(string url)
		{
			var toReturn = await _productViewService.GetContentAsync(ActionContext, BindingContext, User);
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

	    [HttpGet]
	    public async Task<IActionResult> FullReviews(string url, int pageNumber = 1)
	    {
		    var transfer = await _productService.SelectTransferAsync(url, true);

		    var reviewsModel = await GetReviewsModel(new ProductReviewFilter()
		    {
			    IdProduct = transfer.ProductDynamic.Id,
			    Paging = new Paging()
			    {
				    PageIndex = pageNumber,
				    PageItemCount = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT
			    },
			    StatusCode = RecordStatusCode.Active
		    });

		    return View(new FullReviewsModel()
		    {
				ProductPublicId = transfer.ProductDynamic.PublicId,
				ProductTitle = transfer.ProductDynamic.Name,
				ProductSubTitle = transfer.ProductDynamic.Data.SubTitle,
				ProductUrl = transfer.ProductContent.Url,
				Reviews = reviewsModel,
				AverageRatings = await _productReviewService.GetApprovedAverageRatingsAsync(transfer.ProductDynamic.Id)
			});
	    }
	}
}