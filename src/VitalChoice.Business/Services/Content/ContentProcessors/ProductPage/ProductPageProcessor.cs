using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content.ContentProcessors.ProductPage
{
    public class ProductPageParameters : ProcessorModel<ProductContent>
    {
		public const string YoutubeVideoFormat = "https://www.youtube.com/watch?v={0}";
		public const string CategoryBaseUrl = "/products/";

		public RoleType Role { get; set; }

		public bool Preview { get; set; }
    }

    public class ProductPageProcessor : ContentProcessor<TtlProductPageModel, ProductPageParameters>
    {
	    private readonly IProductService _productService;
		private readonly IProductCategoryService _productCategoryService;

		public ProductPageProcessor(IObjectMapper<ProductPageParameters> mapper, IProductService productService, IProductCategoryService productCategoryService) : base(mapper)
        {
	        _productService = productService;
			_productCategoryService = productCategoryService;
		}

	    public override async Task<TtlProductPageModel> ExecuteAsync(ProductPageParameters model)
        {
            if (model?.Model == null)
            {
                throw new ApiException("Invalid product");
            }

            var targetStatuses = new List<RecordStatusCode>() { RecordStatusCode.Active };
            if (model.Model.StatusCode == RecordStatusCode.NotActive)
            {
                if (!model.Preview)
                {
                    return null;
                }
                targetStatuses.Add(RecordStatusCode.NotActive);
            }

			var eProduct = await _productService.SelectAsync(model.Model.Id, true);
		    if (eProduct.Hidden)
		    {
				return null;
			}

		    ProductNavCategoryLite rootNavCategory = null;
            if (eProduct.CategoryIds.Any())
		    {
				rootNavCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(new ProductCategoryLiteFilter()
			    {
				    Statuses = targetStatuses
			    });
		    }

            return PopulateProductPageTemplateModel(model, eProduct, rootNavCategory);
        }

        private bool BuildBreadcrumb(ProductNavCategoryLite rootCategory, int categoryId,
            IList<TtlBreadcrumbItemModel> breadcrumbItems)
        {
            if (rootCategory == null)
                return false;
            if (!rootCategory.SubItems.Any())
            {
                if (!rootCategory.Id.Equals(categoryId))
                {
                    var last = breadcrumbItems.LastOrDefault();
	                if (last != null)
	                {
		                breadcrumbItems.Remove(last);
	                }
                    return false;
                }
                else
                {
                    return true;
                }
            }

            foreach (var subItem in rootCategory.SubItems)
            {
                breadcrumbItems.Add(new TtlBreadcrumbItemModel()
                {
                    Label = subItem.ProductCategory.Name,
                    Url = ProductPageParameters.CategoryBaseUrl + subItem.Url
                });

                if (!subItem.Id.Equals(categoryId))
                {
                    var found = BuildBreadcrumb(subItem, categoryId, breadcrumbItems);
                    if (found)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private TtlProductPageModel PopulateProductPageTemplateModel(ProductPageParameters model, ProductDynamic eProduct,
            ProductNavCategoryLite rootNavCategory = null)
        {
            IList<TtlBreadcrumbItemModel> breadcrumbItems = new List<TtlBreadcrumbItemModel>();

	        var productContent = model.Model;
	        if (eProduct.CategoryIds.Any())
	        {
				BuildBreadcrumb(rootNavCategory, eProduct.CategoryIds.First(), breadcrumbItems);
				breadcrumbItems.Add(new TtlBreadcrumbItemModel()
				{
					Label = eProduct.Name,
					Url = productContent.Url
				});
			}

	        var toReturn = new TtlProductPageModel
	        {
		        Name = eProduct.Name,
		        SubTitle = eProduct.Data.SubTitle,
		        Url = productContent.Url,
		        Image = eProduct.Data.MainProductImage,
		        ShortDescription = eProduct.Data.ShortDescription,
		        SpecialIcon = eProduct.Data.SpecialIcon,
		        BreadcrumbOrderedItems = breadcrumbItems,
		        Skus = eProduct.Skus.Where(x=>!x.Hidden).OrderBy(x => x.Order).Select(x => new TtlProductPageSkuModel()
		        {
			        Code = x.Code,
			        Price = model.Role == RoleType.Retail ? x.Price : x.WholesalePrice,
			        PortionsCount = x.Data.QTY
		        }).ToList(),
		        YoutubeVideos = new List<TtlRelatedYoutubeVideoModel>()
		        {
			        new TtlRelatedYoutubeVideoModel()
			        {
				        Image = eProduct.Data.YouTubeImage1,
				        Text = eProduct.Data.YouTubeText1,
				        Video = string.Format(ProductPageParameters.YoutubeVideoFormat,eProduct.Data.YouTubeVideo1)
			        },
			        new TtlRelatedYoutubeVideoModel()
			        {
				        Image = eProduct.Data.YouTubeImage2,
				        Text = eProduct.Data.YouTubeText2,
				        Video = string.Format(ProductPageParameters.YoutubeVideoFormat,eProduct.Data.YouTubeVideo2)
			        },
			        new TtlRelatedYoutubeVideoModel()
			        {
				        Image = eProduct.Data.YouTubeImage3,
				        Text = eProduct.Data.YouTubeText3,
				        Video = string.Format(ProductPageParameters.YoutubeVideoFormat,eProduct.Data.YouTubeVideo3)
			        },
					new TtlRelatedYoutubeVideoModel()
					{
						Image = eProduct.Data.YouTubeImage4,
						Text = eProduct.Data.YouTubeText4,
						Video = string.Format(ProductPageParameters.YoutubeVideoFormat,eProduct.Data.YouTubeVideo4) 
					}
				},
		        CrossSells = new List<TtlCrossSellProductModel>()
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
		        },
		        DescriptionTab = new TtlProductPageTabContent()
		        {
			        TitleOverride = eProduct.Data.DescriptionTitleOverride,
			        Content = eProduct.Data.Description,
			        Hidden = eProduct.Data.DescriptionHide
		        },
	        };

	        if (eProduct.DictionaryData.ContainsKey("IngredientsTitleOverride"))
	        {
		        toReturn.IngredientsTab = new TtlProductPageTabContent()
		        {
			        TitleOverride = eProduct.Data.IngredientsTitleOverride,
			        Content = eProduct.Data.Ingredients,
			        Hidden = eProduct.Data.IngredientsHide
		        };
	        }

			if (eProduct.DictionaryData.ContainsKey("RecipesTitleOverride"))
			{
				toReturn.RecipesTab = new TtlProductPageTabContent()
				{
					TitleOverride = eProduct.Data.RecipesTitleOverride,
					Content = eProduct.Data.Recipes,
					Hidden = eProduct.Data.RecipesHide
				};
			}

			if (eProduct.DictionaryData.ContainsKey("ServingTitleOverride"))
			{
				toReturn.ServingTab = new TtlProductPageTabContent()
				{
					TitleOverride = eProduct.Data.ServingTitleOverride,
					Content = eProduct.Data.Serving,
					Hidden = eProduct.Data.ServingHide
				};
			}

			if (eProduct.DictionaryData.ContainsKey("ShippingTitleOverride"))
			{
				toReturn.ShippingTab = new TtlProductPageTabContent()
				{
					TitleOverride = eProduct.Data.ShippingTitleOverride,
					Content = eProduct.Data.Shipping,
					Hidden = eProduct.Data.ShippingHide
				};
			}

	        var bestValuedSku =
		        toReturn.Skus.Where(z => z.PortionsCount != 0).FirstOrDefault(x => x.Price/x.PortionsCount == toReturn.Skus.Where(z=>z.PortionsCount != 0).Max(y => y.Price/x.PortionsCount));
	        if (bestValuedSku != null)
	        {
				bestValuedSku.BestValue = true;
			}

            return toReturn;
        }

        public override string ResultName => "ProductPage";
    }
}