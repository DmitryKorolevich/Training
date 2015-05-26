﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
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
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public CategoryService(IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<RecipeToContentCategory> recipeToContentCategory, IRepositoryAsync<Recipe> recipeRepository,
            IRepositoryAsync<FAQToContentCategory> faqToContentCategory, IRepositoryAsync<FAQ> faqRepository,
            IRepositoryAsync<ArticleToContentCategory> articleToContentCategory, IRepositoryAsync<Article> articleRepository)
        {
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.recipeToContentCategory = recipeToContentCategory;
            this.recipeRepository = recipeRepository;
            this.faqToContentCategory = faqToContentCategory;
            this.faqRepository = faqRepository;
            this.articleToContentCategory = articleToContentCategory;
            this.articleRepository = articleRepository;
            logger = LoggerService.GetDefault();
        }

        public async Task<ContentCategory> GetCategoriesTreeAsync(CategoryTreeFilter filter)
        {
            ContentCategory toReturn = null;
            var query = new CategoryQuery().WithType(filter.Type).NotDeleted().WithStatus(filter.Status);
            List<ContentCategory> categories = (await contentCategoryRepository.Query(query).SelectAsync(false)).ToList();

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
            bool toReturn = false;
            if(category==null)
            {
                throw new ArgumentException();
            }

            var query = new CategoryQuery().WithType(category.Type).NotDeleted();
            List<ContentCategory> dbCategories = (await contentCategoryRepository.Query(query).SelectAsync()).ToList();
            category.SetSubCategoriesOrder();

            foreach (var dbCategory in dbCategories)
            {
                ContentCategory uiCategory = FindUICategory(category,dbCategory.Id);
                if(uiCategory!=null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await contentCategoryRepository.UpdateAsync(dbCategory);
                }
            }
            
            toReturn = true;

            return toReturn;
        }

        public async Task<ContentCategory> GetCategoryAsync(int id)
        {
            CategoryQuery query = new CategoryQuery().WithId(id).NotDeleted();
            var toReturn = (await contentCategoryRepository.Query(query).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<ContentCategory> UpdateCategoryAsync(ContentCategory model)
        {
            ContentCategory dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new ContentCategory();
                dbItem.ParentId = model.ParentId;
                if(!dbItem.ParentId.HasValue)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }
                else
                {
                    var parentExist = await contentCategoryRepository.Query(p => p.Id == dbItem.ParentId.Value && p.Type == model.Type &&
                        p.StatusCode !=RecordStatusCode.Deleted).SelectAnyAsync();
                    if(!parentExist)
                    {
                        throw new AppValidationException("The category with the given parent id doesn't exist.");
                    }

                    var subCategories = (await contentCategoryRepository.Query(p => p.ParentId == dbItem.ParentId.Value && p.Type == model.Type &&
                       p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
                    if (subCategories.Count != 0)
                    {
                        dbItem.Order = subCategories.Max(p => p.Order)+1;
                    }
                }

                dbItem.Type = model.Type;
                dbItem.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem = new ContentItem();
                    dbItem.ContentItem.Created = DateTime.Now;
                    dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();
                }

                if (model.MasterContentItemId == 0)
                {
                    //set predefined master
                    var contentType = (await contentTypeRepository.Query(p => p.Id == (int)dbItem.Type).SelectAsync(false)).FirstOrDefault();
                    if(contentType != null && contentType.DefaultMasterContentItemId.HasValue)
                    {
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
                dbItem = (await contentCategoryRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                    SelectAsync()).FirstOrDefault();
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
                var urlDublicatesExist = await contentCategoryRepository.Query(p => p.Url == model.Url && p.Type == model.Type && p.Id != dbItem.Id
                    && p.StatusCode!=RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url", "Category with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
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
                    dbItem = await contentCategoryRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await contentCategoryRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await contentCategoryRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                //Check sub categories
                string message = String.Empty;
                CategoryQuery query = new CategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await contentCategoryRepository.Query(query).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    message += "Category with subcategories can't be deleted. " + Environment.NewLine;
                }

                if (dbItem.Type == ContentType.RecipeCategory)
                {
                    var categories = (await recipeToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false)).ToList();
                    RecipeQuery innerQuery = new RecipeQuery().WithIds(categories.Select(p => p.RecipeId).ToList()).NotDeleted();
                    var exist = (await recipeRepository.Query(innerQuery).SelectAnyAsync());
                    if (exist)
                    {
                        message += "Category with recipes can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.ArticleCategory)
                {
                    var categories = (await articleToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false)).ToList();
                    ArticleQuery innerQuery = new ArticleQuery().WithIds(categories.Select(p => p.ArticleId).ToList()).NotDeleted();
                    var exist = (await articleRepository.Query(innerQuery).SelectAnyAsync());
                    if (exist)
                    {
                        message += "Category with articles can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.FAQCategory)
                {
                    var categories = (await faqToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false)).ToList();
                    FAQQuery innerQuery = new FAQQuery().WithIds(categories.Select(p => p.FAQId).ToList()).NotDeleted();
                    var exist = (await faqRepository.Query(innerQuery).SelectAnyAsync());
                    if (exist)
                    {
                        message += "Category with faqs can't be deleted. " + Environment.NewLine;
                    }
                }

                if (dbItem.Type == ContentType.ContentPageCategory)
                {
                    var categories = (await contentPageToContentCategory.Query(p => p.ContentCategoryId == id).SelectAsync(false)).ToList();
                    ContentPageQuery innerQuery = new ContentPageQuery().WithIds(categories.Select(p => p.ContentPageId).ToList()).NotDeleted();
                    var exist = (await contentPageRepository.Query(innerQuery).SelectAnyAsync());
                    if (exist)
                    {
                        message += "Category with content pages can't be deleted. " + Environment.NewLine;
                    }
                }

                if (!String.IsNullOrEmpty(message))
                {
                    throw new AppValidationException(message);
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await contentCategoryRepository.UpdateAsync(dbItem);

                try
                {
                    templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
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
                foreach (var subCategory in uiRoot.SubCategories)
                {
                    var res = FindUICategory(subCategory, id);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}