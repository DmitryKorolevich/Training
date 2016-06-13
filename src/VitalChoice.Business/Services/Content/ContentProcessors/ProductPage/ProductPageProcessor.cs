﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
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

        public ProductPageProcessor(IObjectMapper<ProductPageParameters> mapper, IProductCategoryService productCategoryService,
            IProductReviewService productReviewService, IRecipeService recipeService)
            : base(mapper)
        {
            _productCategoryService = productCategoryService;
            _productReviewService = productReviewService;
            _recipeService = recipeService;
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

            var targetStatuses = new List<RecordStatusCode>() {RecordStatusCode.Active};
            if (viewContext.Entity.StatusCode == RecordStatusCode.NotActive)
            {
                if (!viewContext.Parameters.Preview)
                {
                    throw new ApiException("Product not found", HttpStatusCode.NotFound);
                }
                targetStatuses.Add(RecordStatusCode.NotActive);
            }

            var customerVisibility = GetCustomerVisibility(viewContext);

            var eProduct = viewContext.Parameters.Product;
            if (!eProduct.IdVisibility.HasValue || 
                !customerVisibility.Contains(eProduct.IdVisibility.Value) || 
                targetStatuses.All(x => x != (RecordStatusCode) eProduct.StatusCode))
            {
                if (!viewContext.Parameters.Preview)
                {
                    throw new ApiException("Product not found", HttpStatusCode.NotFound);
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

            return
                await
                    PopulateProductPageTemplateModel(viewContext, rootAllCategory, lastProductReviews, reviewsCount, ratingsAverage, targetStatuses, 
                    viewContext.Parameters.cat);
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
                if (!string.IsNullOrEmpty(eProduct.SafeData.SubTitle))
                {
                    breadcrumbItems.Last().Label += " " + eProduct.Data.SubTitle;
                }
            }

            var toReturn = new TtlProductPageModel();
            toReturn.ProductPublicId = eProduct.PublicId;
            toReturn.Name = eProduct.Name;
            toReturn.SubTitle = eProduct.Data.SubTitle;
            toReturn.Url = productContent.Url;
            toReturn.Image = eProduct.Data.MainProductImage;
            toReturn.ShortDescription = eProduct.Data.ShortDescription;
            toReturn.SpecialIcon = eProduct.Data.SpecialIcon;
            toReturn.SubProductGroupName = eProduct.Data.SubProductGroupName;
            toReturn.BreadcrumbOrderedItems = breadcrumbItems;
            toReturn.Skus =
                eProduct.Skus.Where(x => !x.Hidden && targetStatusCodes.Contains((RecordStatusCode) x.StatusCode))
                    .OrderBy(x => x.Order)
                    .Select(x => new TtlProductPageSkuModel()
                    {
                        Code = x.Code,
                        SalesText = x.Data.SalesText,
                        Price = viewContext.Parameters.Role == RoleType.Wholesale ? x.WholesalePrice : x.Price,
                        PortionsCount = (int?)x.SafeData.QTY ?? 0,
                        InStock =
                            (x.SafeData.DisregardStock != null && x.SafeData.DisregardStock == true) || x.SafeData.DisregardStock == null
                            || ((int?) x.SafeData.Stock ?? 0) > 0,
                        AutoShip = x.SafeData.AutoShipProduct != null ? x.Data.AutoShipProduct : false,
                    }).ToList();
            toReturn.YoutubeVideos = new List<TtlRelatedYoutubeVideoModel>()
            {
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.Data.YouTubeImage1,
                    Text = eProduct.Data.YouTubeText1,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.Data.YouTubeVideo1),
                    VideoId = eProduct.Data.YouTubeVideo1
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.Data.YouTubeImage2,
                    Text = eProduct.Data.YouTubeText2,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.Data.YouTubeVideo2),
                    VideoId = eProduct.Data.YouTubeVideo2
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.Data.YouTubeImage3,
                    Text = eProduct.Data.YouTubeText3,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.Data.YouTubeVideo3),
                    VideoId = eProduct.Data.YouTubeVideo3
                },
                new TtlRelatedYoutubeVideoModel()
                {
                    Image = eProduct.Data.YouTubeImage4,
                    Text = eProduct.Data.YouTubeText4,
                    Video = string.Format(ProductPageParameters.YoutubeVideoFormat, eProduct.Data.YouTubeVideo4),
                    VideoId = eProduct.Data.YouTubeVideo4
                }
            };
            toReturn.CrossSells = new List<TtlCrossSellProductModel>()
            {
                new TtlCrossSellProductModel()
                {
                    Image = eProduct.Data.CrossSellImage1,
                    Url = eProduct.Data.CrossSellUrl1,
                },
                new TtlCrossSellProductModel()
                {
                    Image = eProduct.Data.CrossSellImage2,
                    Url = eProduct.Data.CrossSellUrl2,
                },
                new TtlCrossSellProductModel()
                {
                    Image = eProduct.Data.CrossSellImage3,
                    Url = eProduct.Data.CrossSellUrl3,
                },
                new TtlCrossSellProductModel()
                {
                    Image = eProduct.Data.CrossSellImage4,
                    Url = eProduct.Data.CrossSellUrl4,
                }
            };
            toReturn.DescriptionTab = new TtlProductPageTabModel()
            {
                TitleOverride = eProduct.Data.DescriptionTitleOverride,
                Content = productContent.ContentItem.Description,
                Hidden = eProduct.Data.DescriptionHide
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

            if (eProduct.DictionaryData.ContainsKey("IngredientsTitleOverride"))
            {
                toReturn.IngredientsTab = new TtlProductIngredientsTabModel()
                {
                    TitleOverride = eProduct.Data.IngredientsTitleOverride,
                    Content = eProduct.Data.Ingredients,
                    Hidden = eProduct.Data.IngredientsHide,
                    NutritionalTitle = eProduct.Data.NutritionalTitle,
                    IngredientsTitle = eProduct.Data.IngredientsTitle,
                    ServingSize = eProduct.Data.ServingSize,
                    Servings = eProduct.Data.Servings,
                    Calories = eProduct.Data.Calories,
                    CaloriesFromFat = eProduct.Data.CaloriesFromFat,
                    TotalFat = eProduct.Data.TotalFat,
                    TotalFatPercent = eProduct.Data.TotalFatPercent,
                    SaturatedFat = eProduct.Data.SaturatedFat,
                    SaturatedFatPercent = eProduct.Data.SaturatedFatPercent,
                    TransFat = eProduct.Data.TransFat,
                    TransFatPercent = eProduct.Data.TransFatPercent,
                    Cholesterol = eProduct.Data.Cholesterol,
                    CholesterolPercent = eProduct.Data.CholesterolPercent,
                    Sodium = eProduct.Data.Sodium,
                    SodiumPercent = eProduct.Data.SodiumPercent,
                    TotalCarbohydrate = eProduct.Data.TotalCarbohydrate,
                    TotalCarbohydratePercent = eProduct.Data.TotalCarbohydratePercent,
                    DietaryFiber = eProduct.Data.DietaryFiber,
                    DietaryFiberPercent = eProduct.Data.DietaryFiberPercent,
                    Sugars = eProduct.Data.Sugars,
                    SugarsPercent = eProduct.Data.SugarsPercent,
                    Protein = eProduct.Data.Protein,
                    ProteinPercent = eProduct.Data.ProteinPercent,
                    AdditionalNotes = eProduct.Data.AdditionalNotes?.Replace("\n", "<br/>")
                };
            }

            if (eProduct.DictionaryData.ContainsKey("RecipesTitleOverride"))
            {
                var recipes = await _recipeService.GetRecipesAsync(new RecipeListFilter() {ProductId = eProduct.Id});

                toReturn.RecipesTab = new TtlProductRecipesTabModel()
                {
                    TitleOverride = eProduct.Data.RecipesTitleOverride,
                    Content = eProduct.Data.Recipes,
                    Hidden = eProduct.Data.RecipesHide,
                    Recipes = recipes.Items.Select(x => new TtlProductRecipeModel()
                    {
                        Name = x.Name,
                        Url = ProductPageParameters.RecipeBaseUrl + x.Url
                    }).ToList(),
                };
            }

            if (eProduct.DictionaryData.ContainsKey("ServingTitleOverride"))
            {
                toReturn.ServingTab = new TtlProductPageTabModel()
                {
                    TitleOverride = eProduct.Data.ServingTitleOverride,
                    Content = eProduct.Data.Serving,
                    Hidden = eProduct.Data.ServingHide
                };
            }

            if (eProduct.DictionaryData.ContainsKey("ShippingTitleOverride"))
            {
                toReturn.ShippingTab = new TtlProductPageTabModel()
                {
                    TitleOverride = eProduct.Data.ShippingTitleOverride,
                    Content = eProduct.Data.Shipping,
                    Hidden = eProduct.Data.ShippingHide
                };
            }

            var bestValuedSku =
                toReturn.Skus.Where(z => z.PortionsCount != 0)
                    .FirstOrDefault(
                        x => x.Price/x.PortionsCount == toReturn.Skus.Where(z => z.PortionsCount != 0).Max(y => y.Price/x.PortionsCount));
            if (bestValuedSku != null)
            {
                bestValuedSku.BestValue = true;
            }

            return toReturn;
        }

        public override string ResultName => "ProductPage";
    }
}