using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.Business.Repositories;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class ProductCategoryParameters
    {
        public IList<CustomerTypeCode> CustomerTypeCodes { get; set; }

        public bool Preview { get; set; }
    }

    public class ProductCategoryProcessor : ContentProcessor<TtlCategoryModel, ProductCategoryParameters, ProductCategoryContent>
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;
        private readonly IRepositoryAsync<ProductCategoryContent> _productCategoryRepository;
        private readonly IRepositoryAsync<ProductContent> _productContentRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryEcommerceRepository;
        private readonly VProductSkuRepository _productRepository;
        private readonly ICustomerService _customerService;
        private readonly ExtendedUserManager _userManager;

        public ProductCategoryProcessor(IObjectMapper<ProductCategoryParameters> mapper,
            IProductCategoryService productCategoryService,
            IProductService productService,
            IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
            IRepositoryAsync<ProductContent> productContentRepository,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoryEcommerceRepository,
            VProductSkuRepository productRepository,
            ICustomerService customerService, ExtendedUserManager userManager) : base(mapper)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
            _productCategoryRepository = productCategoryRepository;
            _productContentRepository = productContentRepository;
            _productToCategoryEcommerceRepository = productToCategoryEcommerceRepository;
            _productRepository = productRepository;
            _customerService = customerService;
            _userManager = userManager;
        }

        private IList<CustomerTypeCode> GetCustomerVisibility(ProcessorViewContext viewContext)
        {
            return viewContext.User.Identity.IsAuthenticated
                ? (viewContext.User.IsInRole(IdentityConstants.WholesaleCustomer)
                    ? new List<CustomerTypeCode>() { CustomerTypeCode.Wholesale, CustomerTypeCode.All }
                    : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All })
                : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All };
        }

        private void DenyAccessIfRetailRuleApplied(ProductCategory category, ProcessorViewContext viewContext, bool wholesaleCustomer)
        {
            if (category.Assigned == CustomerTypeCode.Retail || viewContext.Entity.NavIdVisible == CustomerTypeCode.Retail)
            {
                if (!wholesaleCustomer)
                {
                    return;
                }

                throw new ApiException("Access denied", HttpStatusCode.Forbidden);
            }
        }

        private async Task DenyAccessIfWholesaleRuleApplied(ProductCategory category, ProcessorViewContext viewContext, bool wholesaleCustomer)
        {
            if (category.Assigned == CustomerTypeCode.Wholesale ||
                viewContext.Entity.NavIdVisible == CustomerTypeCode.Wholesale)
            {
                if (wholesaleCustomer)
                {
			        var customer = await _customerService.SelectAsync(Convert.ToInt32(_userManager.GetUserId(viewContext.User)));

                    var wholesaleActiveCustomer = customer.StatusCode == (int)CustomerStatus.Active;
                    if (wholesaleActiveCustomer)
                    {
                        return;
                    }
                }

                throw new ApiException("Access denied", HttpStatusCode.Forbidden);
            }
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

            var customerVisibility = GetCustomerVisibility(viewContext);

            var wholesaleCustomer = viewContext.User.IsInRole(IdentityConstants.WholesaleCustomer);
            if (category != null)
            {
                await DenyAccessIfWholesaleRuleApplied(category, viewContext, wholesaleCustomer);
                DenyAccessIfRetailRuleApplied(category, viewContext, wholesaleCustomer);

                var visibleForCustomerTypes = customerVisibility.Cast<CustomerTypeCode?>().ToList();

                foreach (var subCategory in category.SubCategories)
                {
                    var subCategoryContent = await
                        _productCategoryRepository.Query(
                            p => p.Id == subCategory.Id && p.NavIdVisible != null && visibleForCustomerTypes.Contains(p.NavIdVisible))
                            .SelectFirstOrDefaultAsync(false);
                    if (subCategoryContent != null)
                    {
                        subCategoryContent.ProductCategory = subCategory;

                        subCategoriesContent.Add(subCategoryContent);
                    }
                }
            }

            var productIds =
                (await
                    _productToCategoryEcommerceRepository.Query(x => x.IdCategory == viewContext.Entity.Id).SelectAsync(false))
                    .OrderBy(p=>p.Order)
                    .Select(x => x.IdProduct).ToList();

            IList<VProductSku> products = new List<VProductSku>();
            IList<ProductContent> productContents = null;
            if (productIds.Count > 0)
            {
                var dbProducts =
                    (await _productRepository.GetProductsAsync(new VProductSkuFilter() { IdProducts = productIds }, false)).Items;
                dbProducts = dbProducts.Where(x => targetStatuses.Contains(x.StatusCode)).ToList();

                //order products
                foreach (var productId in productIds)
                {
                    var dbProductGroup = dbProducts.Where(p => p.IdProduct == productId).ToList();
                    dbProductGroup = dbProductGroup.OrderBy(p => p.SkuOrder).ToList();
                    if (dbProductGroup.Any())
                    {
                        products.AddRange(dbProductGroup);
                    }
                }

                productContents = (await _productContentRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false)).ToList();
            }

            var allRootCategory =
                await _productCategoryService.GetLiteCategoriesTreeAsync(rootCategory, new ProductCategoryLiteFilter()
                {
                    Statuses = targetStatuses,
                    ShowAll = true,
                });

            var rootNavCategory = GetFilteredByVisibilityCategories(allRootCategory, customerVisibility);

            var toReturn = PopulateCategoryTemplateModel(viewContext.Entity, customerVisibility, 
                subCategoriesContent, products, productContents, rootNavCategory, allRootCategory, wholesaleCustomer, targetStatuses);

            if (toReturn.SubCategories.Count == 0 && toReturn.Products.Count == 1)
            {
                viewContext.CommandOptions.RedirectUrl = "/product/" + toReturn.Products.First().Url;
            }

            return toReturn;
        }

        private ProductNavCategoryLite GetFilteredByVisibilityCategories(ProductNavCategoryLite navCategory, IList<CustomerTypeCode> visibility)
        {
            if (navCategory != null && (!navCategory.ProductCategory.ParentId.HasValue ||
                (navCategory.NavIdVisible.HasValue && visibility.Contains(navCategory.NavIdVisible.Value))))
            {
                ProductNavCategoryLite toReturn = new ProductNavCategoryLite();
                toReturn.Id = navCategory.Id;
                toReturn.ProductCategory = navCategory.ProductCategory;
                toReturn.NavLabel = navCategory.NavLabel;
                toReturn.NavIdVisible = navCategory.NavIdVisible;
                toReturn.Url = navCategory.Url;
                toReturn.SubItems = new List<ProductNavCategoryLite>();
                foreach (var productNavCategoryLite in navCategory.SubItems)
                {
                    var item = GetFilteredByVisibilityCategories(productNavCategoryLite, visibility);
                    if (item != null)
                    {
                        toReturn.SubItems.Add(item);
                    }
                }
                return toReturn;
            }
            return null;
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
                            Url = currentCategory.Url
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

        private IList<TtlSidebarMenuItemModel> ConvertToSideMenuModelLevel(
            IList<ProductNavCategoryLite> productCategoryLites)
        {
            return productCategoryLites?.Select(x => new TtlSidebarMenuItemModel()
            {
                Label = !string.IsNullOrWhiteSpace(x.NavLabel) ? x.NavLabel : x.ProductCategory.Name,
                Url = x.Url,
                SubItems = ConvertToSideMenuModelLevel(x.SubItems)
            }).ToList();
        }

        private TtlCategoryModel PopulateCategoryTemplateModel(ProductCategoryContent productCategoryContent, IList<CustomerTypeCode> customerVisibility,
            IList<ProductCategoryContent> subProductCategoryContent = null, IList<VProductSku> skuProducts = null, IList<ProductContent> productContents = null,
            ProductNavCategoryLite rootNavCategory = null, ProductNavCategoryLite rootAllCategory = null, bool? wholesaleCustomer = null,
            List<RecordStatusCode> targetStatuses=null)
        {
            IList<TtlBreadcrumbItemModel> breadcrumbItems = new List<TtlBreadcrumbItemModel>();
            BuildBreadcrumb(rootAllCategory, rootAllCategory, productCategoryContent.Id, breadcrumbItems);
            breadcrumbItems = breadcrumbItems.Reverse().ToList();

            var products = skuProducts?.GroupBy(p => p.IdProduct).Select(g => new VProductSku()
            {
                IdProduct = g.Key,
                Name = g.Min(p => p.Name),
                SubTitle = g.Min(p => p.SubTitle),
                Thumbnail = g.Min(p => p.Thumbnail),
                TaxCode = g.Min(p => p.TaxCode),
                StatusCode = g.Min(p => p.StatusCode),
                IdVisibility = g.Min(p => p.IdVisibility),
                DateEdited = g.Min(p => p.DateEdited),
                IdEditedBy = g.Min(p => p.IdEditedBy),
                IdProductType = g.Min(p => p.IdProductType)
            }).ToList();
            
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
                ViewType = (int)productCategoryContent.ViewType,
                SubCategories = subProductCategoryContent?.Select(x => PopulateCategoryTemplateModel(x, customerVisibility)).ToList(),
                Products = products?.Where(x => x.IdVisibility.HasValue && customerVisibility.Contains(x.IdVisibility.Value)).Select(x => new TtlCategoryProductModel
                {
                    Id = x.IdProduct,
                    Name = x.Name,
                    Thumbnail = x.Thumbnail,
                    SubTitle = x.SubTitle,
                }).ToList(),
                Skus = skuProducts?.Where(x => 
                    x.IdVisibility.HasValue && customerVisibility.Contains(x.IdVisibility.Value) && x.SkuId.HasValue && x.SkuHidden.HasValue
                    && !x.SkuHidden.Value && targetStatuses.Contains((RecordStatusCode)x.SkuStatusCode)
                    ).Select(x => new TtlCategorySkuModel
                {
                    IdProduct = x.IdProduct,
                    IdSku = x.SkuId.Value,
                    Code = x.Code,
                    Name = x.Name,
                    Thumbnail = x.Thumbnail,
                    SubTitle = x.SubTitle,
                    ShortDescription = x.ShortDescription,
                    Price = wholesaleCustomer==true ? x.WholesalePrice ?? 0 : x.Price ?? 0,
                    InStock = x.IdProductType == ProductType.EGс || x.IdProductType == ProductType.Gc ||
                            (x.DisregardStock ?? false) || (x.Stock ?? 0) > 0
                }).ToList(),
                SideMenuItems = ConvertToSideMenuModelLevel(rootNavCategory?.SubItems),
                BreadcrumbOrderedItems = breadcrumbItems
            };

            if (toReturn.Products != null && productContents != null)
            {
                foreach (var product in toReturn.Products)
                {
                    var productContent = productContents.FirstOrDefault(p => p.Id == product.Id);
                    if (productContent != null)
                    {
                        product.Url = productContent.Url + "?cat=" + productCategoryContent.Id;
                    }
                }
            }
            if (toReturn.Skus != null && productContents != null)
            {
                foreach (var product in toReturn.Skus)
                {
                    var productContent = productContents.FirstOrDefault(p => p.Id == product.IdProduct);
                    if (productContent != null)
                    {
                        product.Url = productContent.Url + "?cat=" + productCategoryContent.Id;
                    }
                }
            }

            return toReturn;
        }

        public override string ResultName => "ProductCategory";
    }
}