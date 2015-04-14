using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Domain.Entities.Content;
using Microsoft.Data.Entity;
using VitalChoice.Infrastructure.Context;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.Localization;
using System;
using System.Threading;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Domain.Constants;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;
using VitalChoice.Business.Services.Contracts.Content;

namespace VitalChoice.Business.Services.Impl.Content
{
	public class ContentViewService : IContentViewService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IContentProcessorsService contentProcessorsService;
	    private readonly ITtlGlobalCache _templatesCache;
	    private readonly ILogger _logger;

        public ContentViewService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,IRepositoryAsync<Recipe> recipeRepository,
            IContentProcessorsService contentProcessorsService, ITtlGlobalCache templatesCache)
		{
            this.masterContentItemRepository = masterContentItemRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentProcessorsService = contentProcessorsService;
            this.recipeRepository = recipeRepository;
            _templatesCache = templatesCache;
            _logger = LoggerService.GetDefault();
		}

        #region Public

        public async Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, Dictionary<string, object> parameters, string categoryUrl = null)
        {
            ContentCategory category;
            //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!String.IsNullOrEmpty(categoryUrl))
            {
                category = (await contentCategoryRepository.Query(p => p.Url == categoryUrl && p.Type==type &&
                    p.StatusCode!=RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                    ThenInclude(p=>p.MasterContentItemToContentProcessors).ThenInclude(p=>p.ContentProcessor).
                    SelectAsync(false)).FirstOrDefault();
            }
            else
            {
                category = (await contentCategoryRepository.Query(p => !p.ParentId.HasValue && p.Type==type).Include(p => p.MasterContentItem).
                    ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                    SelectAsync(false)).FirstOrDefault();
            }

            if (category == null)
            {
                _logger.LogInformation("The category {0} could not be found", categoryUrl);
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return null;
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
        
        public async Task<ExecutedContentItem> GetContentItemContentAsync(ContentType type, Dictionary<string, object> parameters, string contentDataItemUrl)
        {
            ContentDataItem contentDataItem = null;
            switch(type)
            {
                case ContentType.Recipe:
                    contentDataItem = (await recipeRepository.Query(p => p.Url == contentDataItemUrl && p.StatusCode
                    !=RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
            }

            if (contentDataItem == null)
            {
                _logger.LogInformation("The content item {0} couldn't be found", contentDataItemUrl);
                return null;
            }
            contentDataItem.ContentItem = (await contentItemRepository.Query(p => p.Id == contentDataItem.ContentItemId).Include(p => p.ContentItemToContentProcessors).
                ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();

            if (contentDataItem.ContentItem == null) {
                _logger.LogError("The content item {0} have no template", contentDataItemUrl);
                return null;
            }
            ITtlTemplate template;
            try
            {
                template = _templatesCache.GetOrCreateTemplate(contentDataItem.MasterContentItem.Template,
                    contentDataItem.ContentItem.Template, contentDataItem.ContentItem.Updated,
                    contentDataItem.MasterContentItem.Updated,
                    contentDataItem.MasterContentItemId, contentDataItem.ContentItemId);
            }
            catch (Exception e) {
                _logger.LogError(e.ToString());
                return new ExecutedContentItem
                {
                    HTML = (e as TemplateCompileException)?.ToString()
                };
            }
            dynamic model = new ExpandoObject();
            model.BodyHtml = contentDataItem.ContentItem.Description;
            foreach (var masterContentItemsToContentItemProcessor in contentDataItem.MasterContentItem.MasterContentItemToContentProcessors)
            {
                var processor = contentProcessorsService.GetContentProcessorByName(masterContentItemsToContentItemProcessor.ContentProcessor.Type);
                model = await processor.ExecuteAsync(model, parameters);
            }
            foreach (var contentItemsToContentItemProcessor in contentDataItem.ContentItem.ContentItemToContentProcessors)
            {
                var processor = contentProcessorsService.GetContentProcessorByName(contentItemsToContentItemProcessor.ContentProcessor.Type);
                model = await processor.ExecuteAsync(model, parameters);
            }

            var generatedHtml = template.Generate(model);

            var toReturn = new ExecutedContentItem
            {
                HTML = generatedHtml,
                Title = contentDataItem.ContentItem.Title,
                MetaDescription = contentDataItem.ContentItem.MetaDescription,
                MetaKeywords = contentDataItem.ContentItem.MetaKeywords,
            };

            return toReturn;
        }

	    public async Task<ContentItem> UpdateContentItemAsync(ContentItem itemToUpdate)
	    {
	        return await contentItemRepository.UpdateAsync(itemToUpdate);
	    }

	    public async Task<ContentItem> GetContentItemAsync(int id)
	    {
	        return (await contentItemRepository.Query(c => c.Id == id).SelectAsync(false)).FirstOrDefault();
        }

	    public virtual async Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate)
	    {
            return await masterContentItemRepository.UpdateAsync(itemToUpdate);
        }

	    public virtual async Task<MasterContentItem> GetMasterContentItemAsync(int id)
	    {
	        return (await masterContentItemRepository.Query(m => m.Id == id).SelectAsync(false)).FirstOrDefault();
	    }

	    #endregion

        #region Private

        private async Task<ContentCategory> TryLoadParentTemplate(ContentCategory category)
        {
            if(category.ParentId.HasValue)
            {
                ContentCategory parentCategory;
                using (var uof = new VitalChoiceUnitOfWork())
                {
                    parentCategory = (await uof.RepositoryAsync<ContentCategory>().Query(p => p.Id == category.ParentId.Value).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
                }
                if(parentCategory?.ContentItemId != null)
                {
                    parentCategory.ContentItem = (await contentItemRepository.Query(p => p.Id == parentCategory.ContentItemId).Include(p => p.ContentItemToContentProcessors).
                        ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
                    category.ContentItem = parentCategory.ContentItem;
                    category.ContentItemId = parentCategory.ContentItemId;
                    category.MasterContentItem = parentCategory.MasterContentItem;
                    category.MasterContentItemId = parentCategory.MasterContentItemId;
                }
                else
                {
                    category = await TryLoadParentTemplate(category);
                }
            }
            return category;
        }

        #endregion
    }
}
