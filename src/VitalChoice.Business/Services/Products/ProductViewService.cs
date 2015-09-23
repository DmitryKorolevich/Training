using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Infrastructure.Cache;
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
	    private readonly ILogger _logger;

	    public ProductViewService(IRepositoryAsync<MasterContentItem> masterContentItemRepository,
	        IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
	        IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository,
	        IRepositoryAsync<ContentItem> contentItemRepository, IContentProcessorsService contentProcessorsService,
	        ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider)
	    {
	        this.masterContentItemRepository = masterContentItemRepository;
	        this.productCategoryRepository = productCategoryRepository;
	        this.productCategoryEcommerceRepository = productCategoryEcommerceRepository;
	        this.contentItemRepository = contentItemRepository;
	        this.contentProcessorsService = contentProcessorsService;
	        _templatesCache = templatesCache;
	        _logger = loggerProvider.CreateLoggerDefault();
	    }

	    #region Public

        public async Task<ExecutedContentItem> GetProductCategoryContentAsync(Dictionary<string, object> parameters, string categoryUrl = null)
        {
            ProductCategoryContent category=null;
            //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!String.IsNullOrEmpty(categoryUrl))
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
                    if (category != null)
                    {
                        category.Set(categoryEcommerce);
                    }
                }
            }

            if (category == null)
            {
                _logger.LogInformation("The category {0} could not be found", categoryUrl);
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return null;
            }

            if (category.StatusCode == RecordStatusCode.NotActive)
            {
                if (!parameters.ContainsKey(ContentConstants.PREVIEW_PARAM))
                {
                    return null;
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
                return new ExecutedContentItem
                {
                    HTML = (e as TemplateCompileException)?.ToString()
                };
            }
            dynamic model = new ExpandoObject();
            model = category;
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

            var toReturn = new ExecutedContentItem
            {
                HTML = generatedHtml,
                Title = category.ContentItem.Title,
                MetaDescription = category.ContentItem.MetaDescription,
                MetaKeywords = category.ContentItem.MetaKeywords,
            };
            
            return toReturn;
        }

	    #endregion
    }
}
