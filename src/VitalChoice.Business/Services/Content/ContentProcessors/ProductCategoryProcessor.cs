using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Transfer.TemplateModels;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class ProductCategoryParameters : ContentFilterModel<ProductCategoryContent>
    {
        public IList<RecordStatusCode> TargetStatuses { get; set; }
        public IList<CustomerTypeCode> CustomerTypeCodes { get; set; }
    }

    public class ProductCategoryProcessor : ContentProcessor<TtlCategoryModel, ProductCategoryParameters>
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IRepositoryAsync<ProductCategoryContent> _productCategoryRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryEcommerceRepository;
        private readonly VProductSkuRepository _productRepository;

        public ProductCategoryProcessor(IObjectMapper<ProductCategoryParameters> mapper,
            IProductCategoryService productCategoryService,
            IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoryEcommerceRepository,
            VProductSkuRepository productRepository) : base(mapper)
        {
            _productCategoryService = productCategoryService;
            _productCategoryRepository = productCategoryRepository;
            _productToCategoryEcommerceRepository = productToCategoryEcommerceRepository;
            _productRepository = productRepository;
        }

        public override async Task<TtlCategoryModel> ExecuteAsync(ProductCategoryParameters model)
        {
            if (model?.Model == null)
            {
                throw new ApiException("Invalid category");
            }

            var rootCategory =
                await
                    _productCategoryService.GetCategoriesTreeAsync(new ProductCategoryTreeFilter()
                    {
                        Statuses = model.TargetStatuses
                    });

            var subCategories = FindTargetCategory(rootCategory, model.Model.Id).SubCategories;

            var subCategoriesContent = new List<ProductCategoryContent>();
            foreach (var subCategory in subCategories)
            {
                var subCategoryContent =
                    (await _productCategoryRepository.Query(p => p.Id == subCategory.Id).SelectAsync(false)).Single();
                subCategoryContent.ProductCategory = subCategory;

                subCategoriesContent.Add(subCategoryContent);
            }

            var productIds =
                (await
                    _productToCategoryEcommerceRepository.Query(x => x.IdCategory == model.Model.Id).SelectAsync(false))
                    .Select(x => x.IdProduct).ToList();

            IList<VProductSku> products = null;
            if (productIds.Any())
            {
                products =
                    (await _productRepository.GetProductsAsync(new VProductSkuFilter() {IdProducts = productIds})).Items;
                products = products.Where(x => model.TargetStatuses.Contains(x.StatusCode)).ToList();
            }

            var rootNavCategory =
                await _productCategoryService.GetLiteCategoriesTreeAsync(rootCategory, new ProductCategoryLiteFilter()
                {
                    Visibility = model.CustomerTypeCodes,
                    Statuses = model.TargetStatuses
                });

            return PopulateCategoryTemplateModel(model.Model, subCategoriesContent, products, rootCategory,
                rootNavCategory);
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

        private bool BuildBreadcrumb(ProductCategory rootCategory, string url,
            IList<TtlCategoryBreadcrumbItemModel> breadcrumbItems)
        {
            if (!rootCategory.SubCategories.Any())
            {
                if (!rootCategory.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
                {
                    breadcrumbItems.Clear();
                    return false;
                }
                else
                {
                    return true;
                }
            }

            foreach (var subItem in rootCategory.SubCategories)
            {
                breadcrumbItems.Add(new TtlCategoryBreadcrumbItemModel()
                {
                    Label = subItem.Name,
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
                Label = x.Label,
                Url = x.Link,
                SubItems = ConvertToSideMenuModelLevel(x.SubItems)
            }).ToList();
        }

        private TtlCategoryModel PopulateCategoryTemplateModel(ProductCategoryContent productCategoryContent,
            IList<ProductCategoryContent> subProductCategoryContent = null, IList<VProductSku> products = null,
            ProductCategory rootCategory = null, ProductNavCategoryLite rootNavCategory = null)
        {
            IList<TtlCategoryBreadcrumbItemModel> breadcrumbItems = null;
            if (rootCategory != null)
            {
                breadcrumbItems = new List<TtlCategoryBreadcrumbItemModel>();
                BuildBreadcrumb(rootCategory, productCategoryContent.Url, breadcrumbItems);
            }

            return new TtlCategoryModel()
            {
                Name = productCategoryContent.Name,
                Url = productCategoryContent.Url,
                Order = productCategoryContent.ProductCategory.Order,
                FileImageSmallUrl = productCategoryContent.FileImageSmallUrl,
                FileImageLargeUrl = productCategoryContent.FileImageLargeUrl,
                LongDescription = productCategoryContent.LongDescription,
                LongDescriptionBottom = productCategoryContent.LongDescriptionBottom,
                SubCategories = subProductCategoryContent?.Select(x => PopulateCategoryTemplateModel(x)).ToList(),
                Products = products?.Select(x => new TtlCategoryProductModel()
                {
                    Name = x.Name,
                    Thumbnail = x.Thumbnail,
                    Url = x.Url
                }).ToList(),
                SideMenuItems = ConvertToSideMenuModelLevel(rootNavCategory?.SubItems),
                BreadcrumbOrderedItems = breadcrumbItems
            };
        }

        public override string ResultName => "ProductCategory";
    }
}