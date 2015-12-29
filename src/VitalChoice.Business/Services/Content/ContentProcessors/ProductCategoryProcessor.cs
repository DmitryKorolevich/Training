﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class ProductCategoryParameters
    {
        public IList<CustomerTypeCode> CustomerTypeCodes { get; set; }

        //[ConvertWith(typeof(StringToBoolConverter))]
        public bool Preview { get; set; }
    }

    public class ProductCategoryProcessor : ContentProcessor<TtlCategoryModel, ProductCategoryParameters, ProductCategoryContent>
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IRepositoryAsync<ProductCategoryContent> _productCategoryRepository;
        private readonly IRepositoryAsync<ProductContent> _productContentRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryEcommerceRepository;
        private readonly VProductSkuRepository _productRepository;

        public ProductCategoryProcessor(IObjectMapper<ProductCategoryParameters> mapper,
            IProductCategoryService productCategoryService,
            IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
            IRepositoryAsync<ProductContent> productContentRepository,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoryEcommerceRepository,
            VProductSkuRepository productRepository) : base(mapper)
        {
            _productCategoryService = productCategoryService;
            _productCategoryRepository = productCategoryRepository;
            _productContentRepository = productContentRepository;
            _productToCategoryEcommerceRepository = productToCategoryEcommerceRepository;
            _productRepository = productRepository;
        }

        protected override async Task<TtlCategoryModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            var targetStatuses = new List<RecordStatusCode>() { RecordStatusCode.Active };
            if (viewContext.Entity.StatusCode == RecordStatusCode.NotActive)
            {
                if (!viewContext.Parameters.Preview)
                {
					throw new ApiException("Category not found", HttpStatusCode.NotFound);
				}
                targetStatuses.Add(RecordStatusCode.NotActive);
            }

            var rootCategory =
                await
                    _productCategoryService.GetCategoriesTreeAsync(new ProductCategoryTreeFilter
                    {
                        Statuses = targetStatuses
                    });

            var category = FindTargetCategory(rootCategory, viewContext.Entity.Id);

            var subCategoriesContent = new List<ProductCategoryContent>();
            if (category != null)
            {
                foreach (var subCategory in category.SubCategories)
                {
                    var subCategoryContent =
                        (await _productCategoryRepository.Query(p => p.Id == subCategory.Id).SelectAsync(false)).Single();
                    subCategoryContent.ProductCategory = subCategory;

                    subCategoriesContent.Add(subCategoryContent);
                }
            }

            var productIds =
                (await
                    _productToCategoryEcommerceRepository.Query(x => x.IdCategory == viewContext.Entity.Id).SelectAsync(false))
                    .Select(x => x.IdProduct).ToList();

            IList<VProductSku> products = null;
            IList<ProductContent> productContents = null;
            if (productIds.Any())
            {
                products =
                    (await _productRepository.GetProductsAsync(new VProductSkuFilter() {IdProducts = productIds})).Items;
                products = products.Where(x => targetStatuses.Contains(x.StatusCode)).ToList();

                 productContents = (await _productContentRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false)).ToList();
            }

            var rootNavCategory =
                await _productCategoryService.GetLiteCategoriesTreeAsync(rootCategory, new ProductCategoryLiteFilter()
                {
                    Visibility = viewContext.Parameters.CustomerTypeCodes,
                    Statuses = targetStatuses
                });

            return PopulateCategoryTemplateModel(viewContext.Entity, subCategoriesContent, products, productContents, rootNavCategory);
        }

        private ProductCategory FindTargetCategory(ProductCategory root, int idToFind)
        {
            if (root.Id == idToFind)
            {
                return root;
            }

            foreach (var subCategory in root.SubCategories)
            {
                if (subCategory.Id == idToFind)
                {
                    return subCategory;
                }
                var target = FindTargetCategory(subCategory, idToFind);
                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }

        private bool BuildBreadcrumb(ProductNavCategoryLite rootCategory, string url,
            IList<TtlBreadcrumbItemModel> breadcrumbItems)
        {
            if (rootCategory == null)
                return false;
            if (!rootCategory.SubItems.Any())
            {
                if (!rootCategory.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
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
                    Url = subItem.Url
                });

                if (!subItem.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
                {
                    var found = BuildBreadcrumb(subItem, url, breadcrumbItems);
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

        private IList<TtlSidebarMenuItemModel> ConvertToSideMenuModelLevel(
            IList<ProductNavCategoryLite> productCategoryLites)
        {
            return productCategoryLites?.Select(x => new TtlSidebarMenuItemModel()
            {
                Label = x.NavLabel,
                Url = x.Url,
                SubItems = ConvertToSideMenuModelLevel(x.SubItems)
            }).ToList();
        }

        private TtlCategoryModel PopulateCategoryTemplateModel(ProductCategoryContent productCategoryContent,
            IList<ProductCategoryContent> subProductCategoryContent = null, IList<VProductSku> products = null, IList<ProductContent> productContents=null,
            ProductNavCategoryLite rootNavCategory = null)
        {
            IList<TtlBreadcrumbItemModel> breadcrumbItems = new List<TtlBreadcrumbItemModel>();
            BuildBreadcrumb(rootNavCategory, productCategoryContent.Url, breadcrumbItems);
            var toReturn = new TtlCategoryModel
            {
                Name = productCategoryContent.ProductCategory.Name,
                Url = productCategoryContent.Url,
                Order = productCategoryContent.ProductCategory.Order,
                FileImageSmallUrl = productCategoryContent.FileImageSmallUrl,
                FileImageLargeUrl = productCategoryContent.FileImageLargeUrl,
                LongDescription = productCategoryContent.LongDescription,
                HideLongDescription = productCategoryContent.HideLongDescription,
                LongDescriptionBottom = productCategoryContent.LongDescriptionBottom,
                HideLongDescriptionBottom = productCategoryContent.HideLongDescriptionBottom,
                SubCategories = subProductCategoryContent?.Select(x => PopulateCategoryTemplateModel(x)).ToList(),
                Products = products?.Where(x=>!x.Hidden).Select(x => new TtlCategoryProductModel
                {
                    Id = x.IdProduct,
                    Name = x.Name,
                    Thumbnail = x.Thumbnail,
                    SubTitle = x.SubTitle,
                }).ToList(),
                SideMenuItems = ConvertToSideMenuModelLevel(rootNavCategory?.SubItems),
                BreadcrumbOrderedItems = breadcrumbItems
            };
            if (toReturn.Products != null && productContents != null)
            {
                foreach (var product in toReturn.Products)
                {
                    var productContent = productContents.FirstOrDefault(p => p.Id == product.Id);
                    if(productContent!=null)
                    {
                        product.Url = productContent.Url;
                    }
                }
            }
            return toReturn;
        }

        public override string ResultName => "ProductCategory";
    }
}