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
using VitalChoice.Business.Services.Contracts.ContentProcessors;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.Localization;
using System;
using Microsoft.Framework.Logging;
using Templates;
using VitalChoice.Domain.Constants;

namespace VitalChoice.Business.Services.Impl
{
	public class ContentService : IContentService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IContentProcessorsService contentProcessorsService;
	    private readonly ITtlGlobalCache _templatesCache;
	    private readonly ILogger _logger;

        public ContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
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
            ExecutedContentItem toReturn = null;
            ContentCategory category = null;
            //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!String.IsNullOrEmpty(categoryUrl))
            {
                category = (await contentCategoryRepository.Query(p => p.Url == categoryUrl && p.Type==type).Include(p => p.MasterContentItem).
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
                return toReturn;
            }
            else
            {
                if (category.ContentItemId.HasValue)
                {
                    category.ContentItem = (await contentItemRepository.Query(p => p.Id == category.ContentItemId).Include(p => p.ContentItemToContentProcessors).
                        ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
                }
            }

            //Try load parent templates if needed
            if(category.ContentItem==null)
            {
                category = await TryLoadParentTemplate(category);
            }

            if (category.ContentItem == null)
            {
                _logger.LogInformation("The category {0} have no content template", categoryUrl);
                return toReturn;
            }
            var template = _templatesCache.GetOrCreateTemplate(category.MasterContentItem.Template,
                category.ContentItem.Template, category.ContentItem.Updated, category.MasterContentItem.Updated,
                category.MasterContentItemId, category.ContentItem.Id);
            dynamic model = new ExpandoObject();
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

            toReturn = new ExecutedContentItem
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
            ExecutedContentItem toReturn = null;
            ContentDataItem contentDataItem = null;
            switch(type)
            {
                case ContentType.Recipe:
                    contentDataItem = (await recipeRepository.Query(p => p.Url == contentDataItemUrl).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
            }

            if (contentDataItem == null)
            {
                //TO DO - write to log. No a certain category
                return toReturn;
            }
            else
            {
                contentDataItem.ContentItem = (await contentItemRepository.Query(p => p.Id == contentDataItem.ContentItemId).Include(p => p.ContentItemToContentProcessors).
                    ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
            }

            var template = _templatesCache.GetOrCreateTemplate(contentDataItem.MasterContentItem.Template,
                contentDataItem.ContentItem.Template, contentDataItem.ContentItem.Updated, contentDataItem.MasterContentItem.Updated,
                contentDataItem.MasterContentItemId, contentDataItem.ContentItemId);

            dynamic model = new ExpandoObject();
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

            //TO DO - execute a certain template

            toReturn = new ExecutedContentItem()
            {
                HTML = "<div>Test HTML - Recipe</div>",
                Title = contentDataItem.ContentItem.Title,
                MetaDescription = contentDataItem.ContentItem.MetaDescription,
                MetaKeywords = contentDataItem.ContentItem.MetaKeywords,
            };

            return toReturn;
        }

        #endregion

        #region Private

        private async Task<ContentCategory> TryLoadParentTemplate(ContentCategory category)
        {
            if(category.ParentId.HasValue)
            {
                ContentCategory parentCategory = null;
                using (var uof = new VitalChoiceUnitOfWork())
                {
                    parentCategory = (await uof.RepositoryAsync<ContentCategory>().Query(p => p.Id == category.ParentId.Value).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).SelectAsync(false)).FirstOrDefault();
                }
                if(parentCategory!=null && parentCategory.ContentItemId.HasValue)
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
