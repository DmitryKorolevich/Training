using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Business.Services.Contracts.Product;

namespace VitalChoice.Business.Services.Impl.Product
{
	public class ProductViewService : IProductViewService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ProductCategory> productCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IContentProcessorsService contentProcessorsService;
	    private readonly ITtlGlobalCache _templatesCache;
	    private readonly ILogger _logger;

        public ProductViewService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<ProductCategory> productCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository, IContentProcessorsService contentProcessorsService, ITtlGlobalCache templatesCache)
		{
            this.masterContentItemRepository = masterContentItemRepository;
            this.productCategoryRepository = productCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentProcessorsService = contentProcessorsService;
            _templatesCache = templatesCache;
            _logger = LoggerService.GetDefault();
		}

        #region Public

        public async Task<ExecutedContentItem> GetProductCategoryContentAsync(Dictionary<string, object> parameters, string categoryUrl = null)
        {
            ProductCategory category;
            //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!String.IsNullOrEmpty(categoryUrl))
            {
                category = (await productCategoryRepository.Query(p => p.Url == categoryUrl && 
                    p.StatusCode!=RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                    ThenInclude(p=>p.MasterContentItemToContentProcessors).ThenInclude(p=>p.ContentProcessor).
                    SelectAsync(false)).FirstOrDefault();
            }
            else
            {
                category = (await productCategoryRepository.Query(p => !p.ParentId.HasValue).Include(p => p.MasterContentItem).
                    ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                    SelectAsync(false)).FirstOrDefault();
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
            model.BodyHtml = category.ContentItem.Description;
            model.LongDescriptionHTML = category.LongDescription;
            model.LongDescriptionBottomHTML = category.LongDescriptionBottom;
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
