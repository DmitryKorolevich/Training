﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepositoryAsync<ProductCategoryContent> productCategoryRepository;
        private readonly IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public ProductCategoryService(IRepositoryAsync<ProductCategoryContent> productCategoryRepository,
            IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, ILoggerProviderExtended loggerProvider)
        {
            this.productCategoryRepository = productCategoryRepository;
            this.productCategoryEcommerceRepository = productCategoryEcommerceRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<ProductCategory> GetCategoriesTreeAsync(ProductCategoryTreeFilter filter)
        {
            ProductCategory toReturn = null;
            var query = new ProductCategoryQuery().NotDeleted().WithStatus(filter.Status);
            List<ProductCategory> categories = await productCategoryEcommerceRepository.Query(query).SelectAsync(false);
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
            List<ProductCategory> dbCategories = await productCategoryEcommerceRepository.Query(query).SelectAsync();
            category.SetSubCategoriesOrder();

            foreach (var dbCategory in dbCategories)
            {
                ProductCategory uiCategory = FindUICategory(category,dbCategory.Id);
                if(uiCategory!=null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await productCategoryEcommerceRepository.UpdateAsync(dbCategory);
                }
            }
            
            toReturn = true;

            return toReturn;
        }

        public async Task<ProductCategoryContent> GetCategoryAsync(int id)
        {
            ProductCategoryContent toReturn = null;
            ProductCategoryQuery query = new ProductCategoryQuery().WithId(id).NotDeleted();
            var categoryEcommerce = (await productCategoryEcommerceRepository.Query(query).SelectAsync(false)).FirstOrDefault();
            if(categoryEcommerce!=null)
            {
                toReturn = (await productCategoryRepository.Query(p=>p.Id== categoryEcommerce.Id).Include(p => p.ContentItem).
                    SelectAsync(false)).FirstOrDefault();
                if (toReturn != null)
                {
                    toReturn.Set(categoryEcommerce);
                }
            }
            return toReturn;
        }

        public async Task<ProductCategoryContent> UpdateCategoryAsync(ProductCategoryContent model)
        {
            ProductCategoryContent dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new ProductCategoryContent {ParentId = model.ParentId};
                if(!dbItem.ParentId.HasValue)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }
                var parentExist = await productCategoryEcommerceRepository.Query(p => p.Id == model.ParentId && 
                                                                                      p.StatusCode !=RecordStatusCode.Deleted).SelectAnyAsync();
                if(!parentExist)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }

                var subCategories =
                    await productCategoryEcommerceRepository.Query(p => p.ParentId == model.ParentId &&
                                                                        p.StatusCode != RecordStatusCode.Deleted)
                        .SelectAsync(false);
                if (subCategories.Count != 0)
                {
                    dbItem.Order = subCategories.Max(p => p.Order)+1;
                }

                dbItem.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    dbItem.ContentItem = new ContentItem {Created = DateTime.Now};
                }

                if (model.MasterContentItemId == 0)
                {
                    //set predefined master
                    var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.ProductCategory).SelectAsync(false)).FirstOrDefault();
                    if(contentType?.DefaultMasterContentItemId != null)
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
                dbItem = await GetCategoryAsync(model.Id);
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var idDbItem = dbItem.Id;
                var urlDublicatesExist = await productCategoryEcommerceRepository.Query(p => p.Url == model.Url && p.Id != idDbItem
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


                if (model.Id == 0)
                {
                    var ecommerceProduct = await productCategoryEcommerceRepository.InsertAsync(new ProductCategory(dbItem));
                    if(ecommerceProduct!=null)
                    {
                        dbItem.Id = ecommerceProduct.Id;
                    }
                }
                else
                {
                    await productCategoryEcommerceRepository.UpdateAsync(new ProductCategory(dbItem));
                }

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
                    dbItem.ContentItem.Updated = DateTime.Now;
                    dbItem.ContentItem.Template = model.ContentItem.Template;
                    dbItem.ContentItem.Description = model.ContentItem.Description;
                    dbItem.ContentItem.Title = model.ContentItem.Title;
                    dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                    dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                }

                if (model.Id == 0)
                {
                    dbItem = await productCategoryRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await productCategoryRepository.UpdateAsync(dbItem);
                    if (dbItem.ContentItem != null)
                    {
                        await contentItemRepository.UpdateAsync(dbItem.ContentItem);
                    }
                }
            }

            return dbItem;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await productCategoryEcommerceRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                //Check sub categories
                string message = String.Empty;
                ProductCategoryQuery query = new ProductCategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await productCategoryEcommerceRepository.Query(query).SelectAnyAsync());
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
                await productCategoryEcommerceRepository.UpdateAsync(dbItem);

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
                return uiRoot.SubCategories.Select(subCategory => FindUICategory(subCategory, id)).FirstOrDefault(res => res != null);
            }

            return null;
        }

        #endregion
    }
}