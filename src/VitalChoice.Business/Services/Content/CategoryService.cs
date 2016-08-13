using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Business.Queries.Contents;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<RecipeToContentCategory> recipeToContentCategory;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IRepositoryAsync<FAQToContentCategory> faqToContentCategory;
        private readonly IRepositoryAsync<FAQ> faqRepository;
        private readonly IRepositoryAsync<ArticleToContentCategory> articleToContentCategory;
        private readonly IRepositoryAsync<Article> articleRepository;
        private readonly IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategory;
        private readonly IRepositoryAsync<ContentPage> contentPageRepository;
        private readonly IObjectLogItemExternalService objectLogItemExternalService;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;
        private readonly IRepositoryAsync<MasterContentItem> _masterContentRepository;

        public CategoryService(IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<RecipeToContentCategory> recipeToContentCategory, IRepositoryAsync<Recipe> recipeRepository,
            IRepositoryAsync<FAQToContentCategory> faqToContentCategory, IRepositoryAsync<FAQ> faqRepository,
            IRepositoryAsync<ArticleToContentCategory> articleToContentCategory,
            IRepositoryAsync<Article> articleRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended logger, 
            IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategory, 
            IRepositoryAsync<ContentPage> contentPageRepository, 
            ITtlGlobalCache templatesCache, IRepositoryAsync<MasterContentItem> masterContentRepository)
        {
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.recipeToContentCategory = recipeToContentCategory;
            this.recipeRepository = recipeRepository;
            this.faqToContentCategory = faqToContentCategory;
            this.faqRepository = faqRepository;
            this.articleToContentCategory = articleToContentCategory;
            this.articleRepository = articleRepository;
            this.contentPageToContentCategory = contentPageToContentCategory;
            this.contentPageRepository = contentPageRepository;
            this.objectLogItemExternalService = objectLogItemExternalService;
            this.templatesCache = templatesCache;
            _masterContentRepository = masterContentRepository;
            this.logger = logger.CreateLogger<CategoryService>();
        }

        public async Task<ContentCategory> GetCategoriesTreeAsync(CategoryTreeFilter filter)
        {
            ContentCategory toReturn = null;
            var query = new CategoryQuery().WithType(filter.Type).NotDeleted().WithStatus(filter.Status);
            var categories = await contentCategoryRepository.Query(query).SelectAsync(false);

            toReturn = categories.FirstOrDefault(p => !p.ParentId.HasValue);
            if (toReturn == null)
            {
                throw new Exception("The root category for the given content type isn't configurated. Please contact support.");
            }

            categories.RemoveAll(p => !p.ParentId.HasValue);
            toReturn.CreateSubCategoriesList(categories);

            return toReturn;
        }

        public async Task<bool> UpdateCategoriesTreeAsync(ContentCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var query = new CategoryQuery().WithType(category.Type).NotDeleted();
            var dbCategories = await contentCategoryRepository.Query(query).SelectAsync(true);
            category.SetSubCategoriesOrder();

            foreach (var dbCategory in dbCategories)
            {
                ContentCategory uiCategory = FindUICategory(category, dbCategory.Id);
                if (uiCategory != null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await contentCategoryRepository.UpdateAsync(dbCategory);
                }
            }

            return true;
        }

        public async Task<ContentCategory> GetCategoryAsync(int id)
        {
            CategoryQuery query = new CategoryQuery().WithId(id).NotDeleted();
            var toReturn = (await contentCategoryRepository.Query(query).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<ContentCategory> GetCategoryByIdOldAsync(int id)
        {
            CategoryQuery query = new CategoryQuery().WithIdOld(id).NotDeleted();
            var toReturn = (await contentCategoryRepository.Query(query).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<int?> GetIdRootCategoryAsync(ContentType type)
        {
            CategoryQuery query = new CategoryQuery().RootCategory().WithType(type).NotDeleted();
            var toReturn = (await contentCategoryRepository.Query(query).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                SelectAsync(false)).FirstOrDefault();
            return toReturn?.Id;
        }

        public async Task<ContentCategory> UpdateCategoryAsync(ContentCategory model)
        {
            ContentCategory dbItem;
            if (model.Id == 0)
            {
                dbItem = new ContentCategory { ParentId = model.ParentId };
                if (!dbItem.ParentId.HasValue)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }
                var parentExist = await contentCategoryRepository.Query(p => p.Id == model.ParentId && p.Type == model.Type &&
                                                                             p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (!parentExist)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }

                var subCategories =
                    await
                        contentCategoryRepository.Query(
                            p =>
                                p.ParentId == model.ParentId && p.Type == model.Type &&
                                p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);
                if (subCategories.Count != 0)
                {
                    dbItem.Order = subCategories.Max(p => p.Order) + 1;
                }

                dbItem.Type = model.Type;
                dbItem.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem = new ContentItem
                    {
                        Created = DateTime.Now,
                        ContentItemToContentProcessors = new List<ContentItemToContentProcessor>()
                    };
                }

                if (model.MasterContentItemId == 0)
                {
                    //set predefined master
                    var contentType = (await contentTypeRepository.Query(p => p.Id == (int)model.Type).SelectFirstOrDefaultAsync(false));
                    if (contentType?.DefaultMasterContentItemId != null)
                    {
                        if (
                            await
                                _masterContentRepository.Query(
                                    m => m.Id == contentType.DefaultMasterContentItemId.Value && m.StatusCode == RecordStatusCode.Deleted)
                                    .SelectAnyAsync())
                        {
                            throw new Exception("The default master template has been removed. Please contact support.");
                        }
                        model.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
                    }
                    else
                    {
                        throw new Exception("The default master template isn't confugurated. Please contact support.");
                    }
                }
            }
            else
            {
                dbItem =
                    await
                        contentCategoryRepository.Query(p => p.Id == model.Id)
                            .Include(p => p.ContentItem)
                            .ThenInclude(p => p.ContentItemToContentProcessors)
                            .SelectFirstOrDefaultAsync(true);
                if (dbItem != null)
                {
                    foreach (var proccesorRef in dbItem.ContentItem.ContentItemToContentProcessors)
                    {
                        await contentItemToContentProcessorRepository.DeleteAsync(proccesorRef.Id);
                    }
                }
            }
            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var idDbItem = dbItem.Id;
                var urlDublicatesExist = await contentCategoryRepository.Query(p => p.Url == model.Url && p.Type == model.Type && p.Id != idDbItem
                    && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url", "Category with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                dbItem.UserId = model.UserId;
                if (model.MasterContentItemId != 0)
                {
                    dbItem.MasterContentItemId = model.MasterContentItemId;
                }
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem.Updated = DateTime.Now;
                    dbItem.ContentItem.Template = model.ContentItem.Template;
                    dbItem.ContentItem.Description = model.ContentItem.Description;
                    dbItem.ContentItem.Title = model.ContentItem.Title;
                    dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                    dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                    dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;
                }

                if (model.Id == 0)
                {
                    await contentCategoryRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    await contentCategoryRepository.UpdateAsync(dbItem);
                }
                
                await objectLogItemExternalService.LogItem(dbItem);
            }

            return dbItem;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await contentCategoryRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
            if (dbItem != null)
            {
                //Check sub categories
                string message = string.Empty;
                CategoryQuery query = new CategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await contentCategoryRepository.Query(query).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    message += "Category with subcategories can't be deleted. " + Environment.NewLine;
                }

                if (dbItem.Type == ContentType.RecipeCategory)
                {
                    var categories =
                        await recipeToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false);
                    var exist = false;
                    if (categories.Count > 0)
                    {
                        RecipeQuery innerQuery = new RecipeQuery().WithIds(categories.Select(p => p.RecipeId).ToArray()).NotDeleted();
                        exist = (await recipeRepository.Query(innerQuery).SelectAnyAsync());
                    }
                    if (exist)
                    {
                        message += "Category with recipes can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.ArticleCategory)
                {
                    var categories =
                        await articleToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false);
                    var exist = false;
                    if (categories.Count > 0)
                    {
                        ArticleQuery innerQuery = new ArticleQuery().WithIds(categories.Select(p => p.ArticleId).ToArray()).NotDeleted();
                        exist = await articleRepository.Query(innerQuery).SelectAnyAsync();
                    }
                    if (exist)
                    {
                        message += "Category with articles can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.FaqCategory)
                {
                    var categories = await faqToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false);
                    var exist = false;
                    if (categories.Count > 0)
                    {
                        FAQQuery innerQuery = new FAQQuery().WithIds(categories.Select(p => p.FAQId).ToArray()).NotDeleted();
                        exist = await faqRepository.Query(innerQuery).SelectAnyAsync();
                    }
                    if (exist)
                    {
                        message += "Category with faqs can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.ContentPageCategory)
                {
                    var categories =
                        await contentPageToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false);
                    var exist = false;
                    if (categories.Count > 0)
                    {
                        ContentPageQuery innerQuery = new ContentPageQuery().WithIds(categories.Select(p => p.ContentPageId).ToArray()).NotDeleted();
                        exist = (await contentPageRepository.Query(innerQuery).SelectAnyAsync());
                    }
                    if (exist)
                    {
                        message += "Category with content pages can't be deleted. " + Environment.NewLine;
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    throw new AppValidationException(message);
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await contentCategoryRepository.UpdateAsync(dbItem);

                try
                {
                    await templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }

        #region Private
        private ContentCategory FindUICategory(ContentCategory uiRoot, int id)
        {
            if (uiRoot.Id == id)
            {
                if (uiRoot.ParentId.HasValue)
                {
                    return uiRoot;
                }
            }
            else
            {
                return uiRoot.SubCategories.Select(subCategory => FindUICategory(subCategory, id)).FirstOrDefault(res => res != null);
            }

            return null;
        }

        #endregion
    }
}