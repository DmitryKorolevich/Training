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
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Business.Services.Contracts.Product;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Impl.Product
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepositoryAsync<ProductCategory> productCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public ProductCategoryService(IRepositoryAsync<ProductCategory> productCategoryRepository, IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository)
        {
            this.productCategoryRepository = productCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            logger = LoggerService.GetDefault();
        }

        public async Task<ProductCategory> GetCategoriesTreeAsync(ProductCategoryTreeFilter filter)
        {
            ProductCategory toReturn = null;
            var query = new ProductCategoryQuery().NotDeleted().WithStatus(filter.Status);
            List<ProductCategory> categories = (await productCategoryRepository.Query(query).SelectAsync(false)).ToList();

            toReturn = categories.FirstOrDefault(p => !p.ParentId.HasValue);
            if (toReturn == null)
            {
                throw new Exception("The root category for the given content type isn't configurated. Please contact support.");
            }

            categories.RemoveAll(p => !p.ParentId.HasValue);
            toReturn.CreateSubCategoriesList(categories);

            return toReturn;
        }

        public async Task<bool> UpdateCategoriesTreeAsync(ProductCategory category)
        {
            bool toReturn = false;
            if(category==null)
            {
                throw new ArgumentException();
            }

            var query = new ProductCategoryQuery().NotDeleted();
            List<ProductCategory> dbCategories = (await productCategoryRepository.Query(query).SelectAsync()).ToList();
            category.SetSubCategoriesOrder();

            foreach (var dbCategory in dbCategories)
            {
                ProductCategory uiCategory = FindUICategory(category,dbCategory.Id);
                if(uiCategory!=null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await productCategoryRepository.UpdateAsync(dbCategory);
                }
            }
            
            toReturn = true;

            return toReturn;
        }

        public async Task<ProductCategory> GetCategoryAsync(int id)
        {
            ProductCategoryQuery query = new ProductCategoryQuery().WithId(id).NotDeleted();
            var toReturn = (await productCategoryRepository.Query(query).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<ProductCategory> UpdateCategoryAsync(ProductCategory model)
        {
            ProductCategory dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new ProductCategory();
                dbItem.ParentId = model.ParentId;
                if(!dbItem.ParentId.HasValue)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }
                else
                {
                    var parentExist = await productCategoryRepository.Query(p => p.Id == dbItem.ParentId.Value && 
                        p.StatusCode !=RecordStatusCode.Deleted).SelectAnyAsync();
                    if(!parentExist)
                    {
                        throw new AppValidationException("The category with the given parent id doesn't exist.");
                    }

                    var subCategories = (await productCategoryRepository.Query(p => p.ParentId == dbItem.ParentId.Value && 
                       p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
                    if (subCategories.Count != 0)
                    {
                        dbItem.Order = subCategories.Max(p => p.Order)+1;
                    }
                }
                
                dbItem.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem = new ContentItem();
                    dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();
                }

                if (model.MasterContentItemId == 0)
                {
                    //set predefined master
                    var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.ProductCategory).SelectAsync(false)).FirstOrDefault();
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
                dbItem = (await productCategoryRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
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
                var urlDublicatesExist = await productCategoryRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id
                    && p.StatusCode!=RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url", "Category with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }
                dbItem.Assigned = model.Assigned;
                dbItem.FileImageSmallUrl = model.FileImageSmallUrl;
                dbItem.FileImageLargeUrl = model.FileImageLargeUrl;
                dbItem.LongDescription = model.LongDescription;
                dbItem.LongDescriptionBottom = model.LongDescriptionBottom;
                if (model.MasterContentItemId != 0)
                {
                    dbItem.MasterContentItemId = model.MasterContentItemId;
                }
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem.Created = DateTime.Now;
                    dbItem.ContentItem.Updated = dbItem.ContentItem.Created;
                    dbItem.ContentItem.Template = model.ContentItem.Template;
                    dbItem.ContentItem.Description = model.ContentItem.Description;
                    dbItem.ContentItem.Title = model.ContentItem.Title;
                    dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                    dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                    dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;
                }

                if (model.Id == 0)
                {
                    dbItem = await productCategoryRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await productCategoryRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await productCategoryRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                //Check sub categories
                string message = String.Empty;
                ProductCategoryQuery query = new ProductCategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await productCategoryRepository.Query(query).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    message += "Category with subcategories can't be deleted. " + Environment.NewLine;
                }

                //TODO: check product references

                if (!String.IsNullOrEmpty(message))
                {
                    throw new AppValidationException(message);
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await productCategoryRepository.UpdateAsync(dbItem);

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

        private ProductCategory FindUICategory(ProductCategory uiRoot, int id)
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