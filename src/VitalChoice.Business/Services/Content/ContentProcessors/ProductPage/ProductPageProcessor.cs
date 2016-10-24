using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;
using ApiException = VitalChoice.Ecommerce.Domain.Exceptions.ApiException;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Business.Services.Content.ContentProcessors.ProductPage
{
    public class ProductPageParameters
    {
        public const string YoutubeVideoFormat = "https://www.youtube.com/watch?v={0}";
        public const string CategoryBaseUrl = "/products/";
        public const string RecipeBaseUrl = "/recipe/";

        public int? cat { get; set; }

        public RoleType? Role { get; set; }

        public ProductDynamic Product { get; set; }

        //[ConvertWith(typeof(StringToBoolConverter))]
        public bool Preview { get; set; }
    }

    public class ProductPageProcessor : ContentProcessor<TtlProductPageModel, ProductPageParameters, ProductContent>
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductReviewService _productReviewService;
        private readonly IRecipeService _recipeService;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;

        public ProductPageProcessor(IObjectMapper<ProductPageParameters> mapper,
            IProductCategoryService productCategoryService,
            IProductReviewService productReviewService,
            IRecipeService recipeService,
            IOptions<AppOptions> appOptions, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDynamicMapper<SkuDynamic, Sku> skuMapper)
            : base(mapper)
        {
            _productCategoryService = productCategoryService;
            _productReviewService = productReviewService;
            _recipeService = recipeService;
            _appOptions = appOptions;
            _productMapper = productMapper;
            _skuMapper = skuMapper;
        }

        private IList<CustomerTypeCode> GetCustomerVisibility(ProcessorViewContext viewContext)
        {
            return viewContext.User.Identity.IsAuthenticated
                ? (viewContext.User.IsInRole(IdentityConstants.WholesaleCustomer)
                    ? new List<CustomerTypeCode>() { CustomerTypeCode.Wholesale, CustomerTypeCode.All }
                    : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All })
                : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All };
        }

        protected override async Task<TtlProductPageModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null || viewContext.Parameters.Product == null)
            {
                throw new ApiException("Invalid product");
            }

            var targetStatuses = new List<RecordStatusCode>() { RecordStatusCode.Active };
            if (viewContext.Entity.StatusCode == RecordStatusCode.NotActive)
            {
                if (!viewContext.Parameters.Preview)
                {
                    throw new NotFoundException("Product not found");
                }
                targetStatuses.Add(RecordStatusCode.NotActive);
            }

            var customerVisibility = GetCustomerVisibility(viewContext);

            var eProduct = viewContext.Parameters.Product;
            if (!eProduct.IdVisibility.HasValue ||
                !customerVisibility.Contains(eProduct.IdVisibility.Value) ||
                targetStatuses.All(x => x != (RecordStatusCode)eProduct.StatusCode))
            {
                if (!viewContext.Parameters.Preview)
                {
                    throw new NotFoundException("Product not found");
                }
            }

            ProductNavCategoryLite rootAllCategory = null;
            if (eProduct.CategoryIds.Count > 0)
            {
                rootAllCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(new ProductCategoryLiteFilter()
                {
                    Statuses = targetStatuses,
                    ShowAll = true,
                });
            }

            var lastProductReviews = await _productReviewService.GetProductReviewsAsync(new ProductReviewFilter()
            {
                IdProduct = eProduct.Id,
                Paging = new Paging()
                {
                    PageIndex = 0,
                    PageItemCount = 3
                },
                StatusCode = RecordStatusCode.Active,
                Sorting = new SortFilter()
                {
                    Path = ProductReviewSortPath.DateCreated,
                    SortOrder = FilterSortOrder.Desc
                }
            });

            var reviewsCount = await _productReviewService.GetApprovedCountAsync(eProduct.Id);
            var ratingsAverage = await _productReviewService.GetApprovedAverageRatingsAsync(eProduct.Id);

            var toReturn =
                await
                    PopulateProductPageTemplateModel(viewContext, rootAllCategory, lastProductReviews, reviewsCount, ratingsAverage, targetStatuses,
                    viewContext.Parameters.cat);

            toReturn.ShowDiscountMessage = viewContext.User.Identity.IsAuthenticated && viewContext.User.IsInRole(IdentityConstants.WholesaleCustomer) &&
                eProduct.IdObjectType == (int)ProductType.Perishable;

            return toReturn;
        }

        private void BuildBreadcrumb(ProductNavCategoryLite rootCategory, ProductNavCategoryLite currentCategory, int categoryId,
            IList<TtlBreadcrumbItemModel> breadcrumbItems)
        {
            if (currentCategory != null)
            {
                if (currentCategory.Id.Equals(categoryId))
                {
                    if (currentCategory.ProductCategory.ParentId.HasValue)
                    {
                        breadcrumbItems.Add(new TtlBreadcrumbItemModel()
                        {
                            Label = currentCategory.ProductCategory.Name,
                            Url = ProductPageParameters.CategoryBaseUrl + currentCategory.Url
                        });

                        BuildBreadcrumb(rootCategory, rootCategory, currentCategory.ProductCategory.ParentId.Value, breadcrumbItems);
                    }
                }
                else
                {
                    foreach (var subItem in currentCategory.SubItems)
                    {
                        BuildBreadcrumb(rootCategory, subItem, categoryId, breadcrumbItems);
                    }
                }
            }
        }

        private async Task<TtlProductPageModel> PopulateProductPageTemplateModel(
            ProcessorViewContext viewContext, ProductNavCategoryLite rootAllCategory,
            PagedList<ProductReview> lastProductReviews, int reviewsCount, double ratingsAverage, IList<RecordStatusCode> targetStatusCodes, int? idCategory)
        {
            IList<TtlBreadcrumbItemModel> breadcrumbItems = new List<TtlBreadcrumbItemModel>();

            var productContent = viewContext.Entity;
            var eProduct = viewContext.Parameters.Product;
            if (eProduct.CategoryIds.Count > 0)
            {
                if (idCategory.HasValue)
                {
                    var foundIdCategory = eProduct.CategoryIds.FirstOrDefault(p => p == idCategory.Value);
                    if (foundIdCategory != 0)
                    {
                        BuildBreadcrumb(rootAllCategory, rootAllCategory, foundIdCategory, breadcrumbItems);
                        breadcrumbItems = breadcrumbItems.Reverse().ToList();
                    }
                }
                breadcrumbItems.Add(new TtlBreadcrumbItemModel()
                {
                    Label = eProduct.Name,
                    Url = productContent.Url
                });
                var subTitle = eProduct.SafeData.SubTitle;
                if (!string.IsNullOrEmpty(subTitle))
                {
                    breadcrumbItems.Last().Label += " " + subTitle;
                }
            }

            //Map

            var toReturn = await _productMapper.ToModelAsync<TtlProductPageModel>(eProduct);
            toReturn.Url = productContent.Url;
            toReturn.BreadcrumbOrderedItems = breadcrumbItems;
            toReturn.Skus = await eProduct.Skus.Where(x => !x.Hidden && targetStatusCodes.Contains((RecordStatusCode) x.StatusCode))
                .OrderBy(x => x.Order)
                .Select(async x =>
                {
                    var item = await _skuMapper.ToModelAsync<TtlProductPageSkuModel>(x);
                    item.Price = viewContext.Parameters.Role == RoleType.Wholesale ? x.WholesalePrice : x.Price;
                    item.InStock =
                        (x.SafeData.DisregardStock != null && x.SafeData.DisregardStock == true) || x.SafeData.DisregardStock == null
                        || ((int?) x.SafeData.Stock ?? 0) > 0;
                    return item;
                }).ToListAsync();
            toReturn.AtLeastOneSkuInStock = toReturn.Skus.Any(p => p.InStock);
            toReturn.YoutubeVideos = new List<TtlRelatedYoutubeVideoModel>()
            {
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.SafeData.YouTubeImage1,
                    Text = eProduct.SafeData.YouTubeText1,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.SafeData.YouTubeVideo1),
                    VideoId = eProduct.SafeData.YouTubeVideo1
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.SafeData.YouTubeImage2,
                    Text = eProduct.SafeData.YouTubeText2,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.SafeData.YouTubeVideo2),
                    VideoId = eProduct.SafeData.YouTubeVideo2
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.SafeData.YouTubeImage3,
                    Text = eProduct.SafeData.YouTubeText3,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.SafeData.YouTubeVideo3),
                    VideoId = eProduct.SafeData.YouTubeVideo3
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.SafeData.YouTubeImage4,
                    Text = eProduct.SafeData.YouTubeText4,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.SafeData.YouTubeVideo4),
                    VideoId = eProduct.SafeData.YouTubeVideo4
                }
            };
            toReturn.CrossSells = new List<TtlCrossSellProductModel>
            {
                new TtlCrossSellProductModel
                {
                    Image = eProduct.SafeData.CrossSellImage1,
                    Url = eProduct.SafeData.CrossSellUrl1,
                },
                new TtlCrossSellProductModel
                {
                    Image = eProduct.SafeData.CrossSellImage2,
                    Url = eProduct.SafeData.CrossSellUrl2,
                },
                new TtlCrossSellProductModel
                {
                    Image = eProduct.SafeData.CrossSellImage3,
                    Url = eProduct.SafeData.CrossSellUrl3,
                },
                new TtlCrossSellProductModel
                {
                    Image = eProduct.SafeData.CrossSellImage4,
                    Url = eProduct.SafeData.CrossSellUrl4,
                }
            };
            toReturn.DescriptionTab = new TtlProductPageTabModel()
            {
                TitleOverride = eProduct.SafeData.DescriptionTitleOverride,
                Content = productContent.ContentItem.Description,
                Hidden = (bool?) eProduct.SafeData.DescriptionHide ?? true
            };
            toReturn.ReviewsTab = new TtlProductReviewsTabModel()
            {
                AverageRatings = ratingsAverage,
                ReviewsCount = reviewsCount,
                Reviews = lastProductReviews.Items?.Select(x => new TtlProductReviewModel()
                {
                    CustomerName = x.CustomerName,
                    DateCreated = x.DateCreated,
                    Review = x.Description,
                    Title = x.Title,
                    Rating = x.Rating
                }).ToList()
            };
            toReturn.IngredientsTab = await _productMapper.ToModelAsync<TtlProductIngredientsTabModel>(eProduct);
            toReturn.IngredientsTab.AdditionalNotes = toReturn.IngredientsTab.AdditionalNotes?.Replace("\n", "<br/>");

            var filter = new RecipeListFilter() {ProductId = eProduct.Id};
            filter.Paging = null;
            filter.StatusCode = RecordStatusCode.Active;
            var recipes = await _recipeService.GetRecipesAsync(filter);

            toReturn.RecipesTab = new TtlProductRecipesTabModel()
            {
                TitleOverride = eProduct.SafeData.RecipesTitleOverride,
                Content = eProduct.SafeData.Recipes,
                Hidden = (bool?)eProduct.SafeData.RecipesHide ?? true,
                Recipes = recipes.Items.Select(x => new TtlProductRecipeModel()
                {
                    Name = x.Name,
                    Url = ProductPageParameters.RecipeBaseUrl + x.Url
                }).ToList(),
            };

            toReturn.ServingTab = new TtlProductPageTabModel()
            {
                TitleOverride = eProduct.SafeData.ServingTitleOverride,
                Content = eProduct.SafeData.Serving,
                Hidden = (bool?)eProduct.SafeData.ServingHide ?? true
            };

            toReturn.ShippingTab = new TtlProductPageTabModel()
            {
                TitleOverride = eProduct.SafeData.ShippingTitleOverride,
                Content = eProduct.SafeData.Shipping,
                Hidden = (bool?)eProduct.SafeData.ShippingHide ?? true
            };

            var bestValuedSku =
                toReturn.Skus.Where(z => z.PortionsCount != 0)
                    .FirstOrDefault(
                        x => x.Price / x.PortionsCount == toReturn.Skus.Where(z => z.PortionsCount != 0).Max(y => y.Price / x.PortionsCount));
            if (bestValuedSku != null)
            {
                bestValuedSku.BestValue = true;
            }

            return toReturn;
        }

        public override string ResultName => "ProductPage";
    }
}