using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Business.Services.Contracts.ContentProcessors;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.Localization;
using System;
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

        public ContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,IRepositoryAsync<Recipe> recipeRepository,
            IContentProcessorsService contentProcessorsService)
		{
            this.masterContentItemRepository = masterContentItemRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentProcessorsService = contentProcessorsService;
            this.recipeRepository = recipeRepository;
        }

        #region Public

        public async Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, string categoryUrl = null)
        {
            ExecutedContentItem toReturn = null;
            ContentCategory category = null;
            //TO DO - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
            if (!String.IsNullOrEmpty(categoryUrl))
            {
                category = (await contentCategoryRepository.Query(p => p.Url == categoryUrl).Include(p => p.MasterContentItem).Include(p => p.ContentItem).
                    ThenInclude(p => p.ContentItemsToContentItemProcessors).ThenInclude(p => p.ContentItemProcessor).SelectAsync(false)).FirstOrDefault();
                if (category!=null && category.MasterContentItem.Type != type)
                {
                    category = null;
                }
            }
            else
            {
                var masterTemplateIds = (await masterContentItemRepository.Query(p => p.Type == ContentType.Recipe).SelectAsync(false)).Select(p => p.Id).ToList();
                category = (await contentCategoryRepository.Query(p => !p.ParentId.HasValue && masterTemplateIds.Contains(p.MasterContentItemId)).Include(p => p.MasterContentItem).Include(p => p.ContentItem).
                    ThenInclude(p => p.ContentItemsToContentItemProcessors).ThenInclude(p => p.ContentItemProcessor).SelectAsync(false)).FirstOrDefault();
            }

            if (category == null)
            {
                //TO DO - write to log. No a certain category
                return toReturn;
            }

            //Try load parent templates if needed
            if(category.ContentItem==null)
            {
                category = await TryLoadParentTemplate(category);
            }

            //TODO: - check complining templates valid date, and recompile if needed

            if (category.ContentItem == null)
            {
                //TODO: - write to log. Category without a template
                return toReturn;
            }

            dynamic model = new ExpandoObject();
            Dictionary<string, object> queryDate = new Dictionary<string, object>();
            queryDate.Add(ContentConstants.CATEGORY_ID, category.Id);
            foreach(var contentItemsToContentItemProcessor in category.ContentItem.ContentItemsToContentItemProcessors)
            {
                var processor = contentProcessorsService.GetContentProcessorByName(contentItemsToContentItemProcessor.ContentItemProcessor.Type);
                model = await processor.ExecuteAsync(model, queryDate);
            }

            //TO DO - execute a certain template

            toReturn = new ExecutedContentItem()
            {
                HTML = "<div>Test HTML - Category</div>",
                Title = category.ContentItem.Title,
                MetaDescription = category.ContentItem.MetaDescription,
                MetaKeywords = category.ContentItem.MetaKeywords,
            };
            
            return toReturn;
        }
        
        public async Task<ExecutedContentItem> GetContentItemContentAsync(ContentType type, string contentDataItemUrl)
        {
            ExecutedContentItem toReturn = null;
            ContentDataItem contentDataItem = null;
            switch(type)
            {
                case ContentType.Recipe:
                    contentDataItem = (await recipeRepository.Query(p => p.Url == contentDataItemUrl).Include(p => p.MasterContentItem).Include(p => p.ContentItem).
                        ThenInclude(p => p.ContentItemsToContentItemProcessors).ThenInclude(p => p.ContentItemProcessor).SelectAsync(false)).FirstOrDefault();
                    break;
            }

            if (contentDataItem == null)
            {
                //TO DO - write to log. No a certain category
                return toReturn;
            }

            //TO DO - check complining templates valid date, and recompile if needed

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
                    parentCategory = (await uof.RepositoryAsync<ContentCategory>().Query(p => p.Id == category.ParentId.Value).Include(p => p.MasterContentItem).Include(p => p.ContentItem).
                    ThenInclude(p => p.ContentItemsToContentItemProcessors).ThenInclude(p => p.ContentItemProcessor).SelectAsync(false)).FirstOrDefault();
                }
                if(parentCategory!=null && parentCategory.ContentItem!=null)
                {
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
