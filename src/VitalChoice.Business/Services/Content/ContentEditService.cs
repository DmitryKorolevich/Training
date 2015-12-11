using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class ContentEditService : IContentEditService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IRepositoryAsync<FAQ> faqRepository;
        private readonly IRepositoryAsync<Article> articleRepository;
        private readonly IRepositoryAsync<ContentPage> contentPageRepository;
	    private readonly ITtlGlobalCache _templatesCache;
	    private readonly ILogger _logger;

	    public ContentEditService(IRepositoryAsync<MasterContentItem> masterContentItemRepository,
	        IRepositoryAsync<ContentCategory> contentCategoryRepository,
	        IRepositoryAsync<ContentItem> contentItemRepository, IRepositoryAsync<Recipe> recipeRepository,
	        IRepositoryAsync<FAQ> faqRepository,
	        IRepositoryAsync<Article> articleRepository, IRepositoryAsync<ContentPage> contentPageRepository,
	        ITtlGlobalCache templatesCache,
	        ILoggerProviderExtended loggerProvider)
	    {
	        this.masterContentItemRepository = masterContentItemRepository;
	        this.contentCategoryRepository = contentCategoryRepository;
	        this.contentItemRepository = contentItemRepository;
	        this.recipeRepository = recipeRepository;
	        this.faqRepository = faqRepository;
	        this.articleRepository = articleRepository;
	        this.contentPageRepository = contentPageRepository;
	        _templatesCache = templatesCache;
	        _logger = loggerProvider.CreateLoggerDefault();
	    }

	    #region Public

        public async Task<ContentViewModel> GetCategoryContentAsync(ContentType type, Dictionary<string, object> parameters, string categoryUrl = null)
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
                return new ContentViewModel
                {
                    Body = (e as TemplateCompileException)?.ToString()
                };
            }
            dynamic model = new ExpandoObject();
            model.BodyHtml = category.ContentItem.Description;
            parameters.Add(ContentConstants.CATEGORY_ID, category.Id);
            //foreach (var masterContentItemsToContentItemProcessor in category.MasterContentItem.MasterContentItemToContentProcessors)
            //{
            //    var processor = contentProcessorsService.GetContentProcessorByName(masterContentItemsToContentItemProcessor.ContentProcessor.Type);
            //    model = await processor.ExecuteAsync(model, parameters);
            //}
            //foreach (var contentItemsToContentItemProcessor in category.ContentItem.ContentItemToContentProcessors)
            //{
            //    var processor = contentProcessorsService.GetContentProcessorByName(contentItemsToContentItemProcessor.ContentProcessor.Type);
            //    model = await processor.ExecuteAsync(model, parameters);
            //}

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
        
        public async Task<ContentViewModel> GetContentItemContentAsync(ContentType type, Dictionary<string, object> parameters, string contentDataItemUrl)
        {
            ContentDataItem contentDataItem = null;
            switch(type)
            {
                case ContentType.Recipe:
                    contentDataItem = (await recipeRepository.Query(p => p.Url == contentDataItemUrl && p.StatusCode!=RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
                case ContentType.Article:
                    contentDataItem = (await articleRepository.Query(p => p.Url == contentDataItemUrl && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
                case ContentType.Faq:
                    contentDataItem = (await faqRepository.Query(p => p.Url == contentDataItemUrl && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
                case ContentType.ContentPage:
                    contentDataItem = (await contentPageRepository.Query(p => p.Url == contentDataItemUrl && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.MasterContentItem).
                        ThenInclude(p => p.MasterContentItemToContentProcessors).ThenInclude(p => p.ContentProcessor).
                        SelectAsync(false)).FirstOrDefault();
                    break;
            }

            if (contentDataItem == null)
            {
                _logger.LogInformation("The content item {0} couldn't be found", contentDataItemUrl);
                return null;
            }
            if (contentDataItem.StatusCode == RecordStatusCode.NotActive)
            {
                if (!parameters.ContainsKey(ContentConstants.PREVIEW_PARAM))
                {
                    return null;
                }
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
                return new ContentViewModel
                {
                    Body = (e as TemplateCompileException)?.ToString()
                };
            }
            dynamic model = new ExpandoObject();
            model.BodyHtml = contentDataItem.ContentItem.Description;
            //foreach (var masterContentItemsToContentItemProcessor in contentDataItem.MasterContentItem.MasterContentItemToContentProcessors)
            //{
            //    var processor = contentProcessorsService.GetContentProcessorByName(masterContentItemsToContentItemProcessor.ContentProcessor.Type);
            //    model = await processor.ExecuteAsync(model, parameters);
            //}
            //foreach (var contentItemsToContentItemProcessor in contentDataItem.ContentItem.ContentItemToContentProcessors)
            //{
            //    var processor = contentProcessorsService.GetContentProcessorByName(contentItemsToContentItemProcessor.ContentProcessor.Type);
            //    model = await processor.ExecuteAsync(model, parameters);
            //}

            var generatedHtml = template.Generate(model);

            var toReturn = new ContentViewModel
            {
                Body = generatedHtml,
                Title = contentDataItem.ContentItem.Title,
                MetaDescription = contentDataItem.ContentItem.MetaDescription,
                MetaKeywords = contentDataItem.ContentItem.MetaKeywords,
            };

            return toReturn;
        }

	    public async Task<ContentItem> UpdateContentItemAsync(ContentItem itemToUpdate)
	    {
	        await contentItemRepository.UpdateAsync(itemToUpdate);
	        return itemToUpdate;
	    }

	    public async Task<ContentItem> GetContentItemAsync(int id)
	    {
	        return (await contentItemRepository.Query(c => c.Id == id).SelectAsync(false)).FirstOrDefault();
        }

	    public virtual async Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate)
	    {
            await masterContentItemRepository.UpdateAsync(itemToUpdate);
            return itemToUpdate;
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
