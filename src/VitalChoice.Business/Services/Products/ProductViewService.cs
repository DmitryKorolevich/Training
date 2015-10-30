using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Transfer.TemplateModels;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
	public class ProductViewService : IProductViewService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository;
        private readonly IRepositoryAsync<ProductCategoryContent> productCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IContentProcessorsService contentProcessorsService;
	    private readonly ITtlGlobalCache _templatesCache;
		private readonly VProductSkuRepository _productRepository;
		private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryEcommerceRepository;
		private readonly IProductCategoryService _productCategoryService;
		private readonly ILogger _logger;

	    public ProductViewService(IRepositoryAsync<MasterContentItem> masterContentItemRepository,
	        IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
	        IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository,
	        IRepositoryAsync<ContentItem> contentItemRepository, IContentProcessorsService contentProcessorsService,
	        ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider, VProductSkuRepository productRepository, IEcommerceRepositoryAsync<ProductToCategory> productToCategoryEcommerceRepository, IProductCategoryService productCategoryService)
	    {
	        this.masterContentItemRepository = masterContentItemRepository;
	        this.productCategoryRepository = productCategoryRepository;
	        this.productCategoryEcommerceRepository = productCategoryEcommerceRepository;
	        this.contentItemRepository = contentItemRepository;
	        this.contentProcessorsService = contentProcessorsService;
	        _templatesCache = templatesCache;
			_productRepository = productRepository;
		    _productToCategoryEcommerceRepository = productToCategoryEcommerceRepository;
		    _productCategoryService = productCategoryService;
		    _logger = loggerProvider.CreateLoggerDefault();
	    }

		private bool BuildBreadcrumb(ProductCategory rootCategory, string url, IList<TtlCategoryBreadcrumbItemModel> breadcrumbItems)
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
				else
				{
					var target = FindTargetCategory(subCategory, idToFind);
					if (target != null)
					{
						return target;
					}
				}
			}

			return null;
		}

		private IList<TtlSidebarMenuItemModel> ConvertToSideMenuModelLevel(IList<ProductNavCategoryLite> productCategoryLites)
		{
			return productCategoryLites?.Select(x => new TtlSidebarMenuItemModel()
			{
				Label = x.Label,
				Url = x.Link,
				SubItems = ConvertToSideMenuModelLevel(x.SubItems)
			}).ToList();
		}

		private TtlCategoryModel PopulateCategoryTemplateModel(ProductCategoryContent productCategoryContent, IList<ProductCategoryContent> subProductCategoryContent = null, IList<VProductSku> products = null, ProductCategory rootCategory = null, ProductNavCategoryLite rootNavCategory = null)
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
				Order = productCategoryContent.Order,
				FileImageSmallUrl = productCategoryContent.FileImageSmallUrl,
				FileImageLargeUrl = productCategoryContent.FileImageLargeUrl,
				LongDescription = productCategoryContent.LongDescription,
				LongDescriptionBottom = productCategoryContent.LongDescriptionBottom,
				SubCategories = subProductCategoryContent?.Select(x=> PopulateCategoryTemplateModel(x)).ToList(),
				Products = products?.Select(x=> new TtlCategoryProductModel()
				{
					Name = x.Name,
					Thumbnail = x.Thumbnail,
					Url = x.Url
				}).ToList(),
				SideMenuItems = ConvertToSideMenuModelLevel(rootNavCategory?.SubItems),
				BreadcrumbOrderedItems = breadcrumbItems
			};
		}

		#region Public

        public async Task<ContentViewModel> GetProductCategoryContentAsync(IList<CustomerTypeCode> customerTypeCodes, Dictionary<string, object> parameters, string categoryUrl = null)
        {
            ProductCategoryContent category=null;
            //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!string.IsNullOrEmpty(categoryUrl))
            {
                var categoryEcommerce = (await productCategoryEcommerceRepository.Query(p => p.Url == categoryUrl &&
                    p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
                if (categoryEcommerce != null)
                {
                    category = (await productCategoryRepository.Query(p=>p.Id== categoryEcommerce.Id).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
                    if(category!=null)
                    {
                        category.Set(categoryEcommerce);
                    }
                }
            }
            else
            {
                var categoryEcommerce = (await productCategoryEcommerceRepository.Query(p => !p.ParentId.HasValue).SelectAsync(false)).FirstOrDefault();
                if (categoryEcommerce != null)
                {
                    category = (await productCategoryRepository.Query(p => p.Id == categoryEcommerce.Id).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
	                category?.Set(categoryEcommerce);
                }
            }

            if (category == null)
            {
                _logger.LogInformation("The category {0} could not be found", categoryUrl);
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return null;
            }

			var targetStatuses = new List<RecordStatusCode>() { RecordStatusCode.Active };
            if (category.StatusCode == RecordStatusCode.NotActive)
            {
                if (!parameters.ContainsKey(ContentConstants.PREVIEW_PARAM))
                {
                    return null;
                }
                else
                {
	                targetStatuses.Add(RecordStatusCode.NotActive);
                }
            }

            //Added this to prevent possible closure edit in multi-threaded requests
            var queryParamCategory = category;
            category.ContentItem = (await contentItemRepository.Query(p => p.Id == queryParamCategory.ContentItemId).Include(p => p.ContentItemToContentProcessors).
                ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
            
            if (category.ContentItem == null)
            {
                _logger.LogError("The category {0} have no template", categoryUrl);
                return null;
            }
            ITtlTemplate template;
            try {
                template = _templatesCache.GetOrCreateTemplate(category.MasterContentItem.Template,
                category.ContentItem.Template, category.ContentItem.Updated, category.MasterContentItem.Updated,
                category.MasterContentItemId, category.ContentItem.Id);
            }
            catch (Exception e) {
                _logger.LogError(e.ToString());
                return new ContentViewModel
                {
                    Body = (e as TemplateCompileException)?.ToString()
                };
            }

			/*model population todo: should be placed in processor and this service must be generic for every content manageable page*/

			var rootCategory = await _productCategoryService.GetCategoriesTreeAsync(new ProductCategoryTreeFilter() { Statuses = targetStatuses });

			var subCategories = FindTargetCategory(rootCategory, category.Id).SubCategories;

			var subCategoriesContent = new List<ProductCategoryContent>();
			foreach (var subCategory in subCategories)
	        {
				var subCategoryContent = (await productCategoryRepository.Query(p => p.Id == subCategory.Id).OrderBy(x=>x.OrderBy(y=>y.Order)).SelectAsync(false)).Single();
				subCategoryContent.Set(subCategory);

				subCategoriesContent.Add(subCategoryContent);
            }

	        var productIds =
		        (await _productToCategoryEcommerceRepository.Query(x => x.IdCategory == category.Id).SelectAsync(false))
			        .Select(x => x.IdProduct).ToList();

	        IList<VProductSku> products = null;
	        if (productIds.Any())
	        {
		        products = (await _productRepository.GetProductsAsync(new VProductSkuFilter() {IdProducts = productIds})).Items;
				products = products.Where(x => targetStatuses.Contains(x.StatusCode)).ToList();
	        }

			var rootNavCategory = await _productCategoryService.GetLiteCategoriesTreeAsync(rootCategory, new ProductCategoryLiteFilter()
			{
				Visibility = customerTypeCodes,
				Statuses = targetStatuses
			});

			var model = PopulateCategoryTemplateModel(category, subCategoriesContent, products, rootCategory, rootNavCategory);

			/*end of model population-------*/

			parameters.Add(ContentConstants.CATEGORY_ID, category.Id);
            foreach (var masterContentItemsToContentItemProcessor in category.MasterContentItem.MasterContentItemToContentProcessors)
            {
                var processor = contentProcessorsService.GetContentProcessorByName(masterContentItemsToContentItemProcessor.ContentProcessor.Type);
                model = await processor.ExecuteAsync(model, parameters);
            }
            foreach (var contentItemsToContentItemProcessor in category.ContentItem.ContentItemToContentProcessors)
            {
                var processor = contentProcessorsService.GetContentProcessorByName(contentItemsToContentItemProcessor.ContentProcessor.Type);
                model = await processor.ExecuteAsync(model, parameters);
            }

            var generatedHtml = template.Generate(model);

            var toReturn = new ContentViewModel
            {
                Body = generatedHtml,
                Title = category.ContentItem.Title,
                MetaDescription = category.ContentItem.MetaDescription,
                MetaKeywords = category.ContentItem.MetaKeywords,
            };
            
            return toReturn;
        }

	    #endregion
    }
}
