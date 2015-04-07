using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Business.Services.Impl.Content.ContentProcessors;
using Templates;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Business.Services.Impl.Content
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public CategoryService(IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository)
        {
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            logger = LoggerService.GetDefault();
        }

        public async Task<ContentCategory> GetCategoriesTreeAsync(ContentType type, RecordStatusCode? status = null)
        {
            ContentCategory toReturn = null;
            var query = new CategoryQuery().WithType(type).NotDeleted().WithStatus(status);
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
            
            foreach(var dbCategory in dbCategories)
            {
                ContentCategory uiCategory = FindUICategory(category,dbCategory.Id);
                if(uiCategory!=null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
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
                    throw new AppValidationException("The root category can't be edited.");
                }
                dbItem.Type = model.Type;
                dbItem.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem = new ContentItem();
                    dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();
                }

                if (model.MasterContentItemId == 0)
                {
                    //set predefined master
                    var parentCategory = (await contentCategoryRepository.Query(p => p.Id == dbItem.ParentId).SelectAsync(false)).FirstOrDefault();
                    if(parentCategory!=null)
                    {
                        model.MasterContentItemId = parentCategory.MasterContentItemId;
                    }
                    else
                    {
                        throw new Exception("The root category for the given content type isn't configurated. Please contact support.");
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
                var urlDublicatesExist = await contentCategoryRepository.Query(p => p.Url == model.Url && p.Type == model.Type && p.Id != dbItem.Id).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Category with the same name is already exist.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                dbItem.MasterContentItemId = model.MasterContentItemId;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem.Created = DateTime.Now;
                    dbItem.ContentItem.Updated = dbItem.ContentItem.Created;
                    dbItem.ContentItem.Template = model.ContentItem.Template;
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
                var subCategoriesExist = (await contentCategoryRepository.Query(p => p.ParentId == id).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    throw new AppValidationException("Category with subcategories can't be deleted.");
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await contentCategoryRepository.UpdateAsync(dbItem);

                if(dbItem.ContentItemId.HasValue)
                {
                    templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId.Value);
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
                if (uiRoot.Parent != null)
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