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
using VitalChoice.Infrastructure.Cache;
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
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, ILoggerProviderExtended loggerProvider, ITtlGlobalCache templatesCache)
        {
            this.productCategoryRepository = productCategoryRepository;
            this.productCategoryEcommerceRepository = productCategoryEcommerceRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.templatesCache = templatesCache;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<ProductCategory> GetCategoriesTreeAsync(ProductCategoryTreeFilter filter)
        {
            ProductCategory toReturn = null;
            var query = new ProductCategoryQuery().NotDeleted().WithStatus(filter.Statuses);
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

        private static void SetSubCategoriesOrder(ProductNavCategoryLite root)
        {
            if (root.SubItems != null)
            {
                for (int i = 0; i < root.SubItems.Count; i++)
                {
                    root.SubItems[i].ProductCategory.Order = i;
                }
            }

            if (root.SubItems != null)
            {
                foreach (var subCategory in root.SubItems)
                {
                    SetSubCategoriesOrder(subCategory);
                }
            }
        }

        public async Task<bool> UpdateCategoriesTreeAsync(ProductNavCategoryLite category)
        {
            if (category == null)
            {
                throw new ArgumentException();
            }

            var query = new ProductCategoryQuery().NotDeleted();
            List<ProductCategory> dbCategories = await productCategoryEcommerceRepository.Query(query).SelectAsync();
            SetSubCategoriesOrder(category);

            foreach (var dbCategory in dbCategories)
            {
                ProductNavCategoryLite uiCategory = FindUICategory(category, dbCategory.Id);
                if(uiCategory!=null)
                {
                    dbCategory.ParentId = uiCategory.ProductCategory.ParentId;
                    dbCategory.Order = uiCategory.ProductCategory.Order;
                    await productCategoryEcommerceRepository.UpdateAsync(dbCategory);
                }
            }

            return true;
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
                    toReturn.ProductCategory = categoryEcommerce;
                }
            }
            return toReturn;
        }

        private async Task UpdateCategory(ProductCategoryContent categoryContent, ProductCategoryContent model)
        {
            var idDbItem = categoryContent.Id;
            var urlDublicatesExist = await productCategoryRepository.Query(p => p.Url == model.Url && p.Id != idDbItem
                && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
            if (urlDublicatesExist)
            {
                throw new AppValidationException("Url", "Category with the same URL already exists, please use a unique URL.");
            }
            var nameDublicatesExist = await productCategoryEcommerceRepository.Query(p => p.Name == model.Name && p.Id != idDbItem
                && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
            if (nameDublicatesExist)
            {
                throw new AppValidationException("Name", "Category with the same Name already exists, please use a unique Name.");
            }

            categoryContent.Name = model.Name;
            categoryContent.Url = model.Url;
            if (model.StatusCode != RecordStatusCode.Deleted)
            {
                categoryContent.StatusCode = model.StatusCode;
            }
            categoryContent.ProductCategory.Assigned = model.ProductCategory.Assigned;


            //if (model.Id == 0)
            //{
            //    var ecommerceProduct = new ProductCategory();
            //    ecommerceProduct.SetContent(categoryContent);
            //    await productCategoryEcommerceRepository.InsertAsync(ecommerceProduct);
            //    categoryContent.Id = ecommerceProduct.Id;
            //}
            //else
            //{
            //    await productCategoryEcommerceRepository.UpdateAsync(categoryContent.EcommerceCategory);
            //}

            categoryContent.FileImageSmallUrl = model.FileImageSmallUrl;
            categoryContent.FileImageLargeUrl = model.FileImageLargeUrl;
            categoryContent.LongDescription = model.LongDescription;
            categoryContent.LongDescriptionBottom = model.LongDescriptionBottom;
            categoryContent.NavLabel = model.NavLabel;
            categoryContent.NavIdVisible = model.NavIdVisible;
            if (model.MasterContentItemId != 0)
            {
                categoryContent.MasterContentItemId = model.MasterContentItemId;
            }
            if (model.ContentItem != null)
            {
                categoryContent.ContentItem.Updated = DateTime.Now;
                categoryContent.ContentItem.Template = model.ContentItem.Template;
                categoryContent.ContentItem.Description = model.ContentItem.Description;
                categoryContent.ContentItem.Title = model.ContentItem.Title;
                categoryContent.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                categoryContent.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
            }

            if (model.Id == 0)
            {
                await productCategoryRepository.InsertGraphAsync(categoryContent);
            }
            else
            {
                await productCategoryRepository.UpdateAsync(categoryContent);
                if (categoryContent.ContentItem != null)
                {
                    await contentItemRepository.UpdateAsync(categoryContent.ContentItem);
                }
            }
        }

        public async Task<ProductCategoryContent> UpdateCategoryAsync(ProductCategoryContent model)
        {
            if (model.Id == 0)
            {
                if (model.ProductCategory.ParentId == null)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }
                var parentExist =
                    await
                        productCategoryEcommerceRepository.Query(
                            p => p.Id == model.ProductCategory.ParentId && p.StatusCode != RecordStatusCode.Deleted)
                            .SelectAnyAsync();
                if(!parentExist)
                {
                    throw new AppValidationException("The category with the given parent id doesn't exist.");
                }

                var subCategories =
                    await
                        productCategoryEcommerceRepository.Query(
                            p =>
                                p.ParentId == model.ProductCategory.ParentId &&
                                p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);
                if (subCategories.Count > 0)
                {
                    model.ProductCategory.Order = subCategories.Max(p => p.Order)+1;
                }

                model.StatusCode = RecordStatusCode.Active;
                if (model.ContentItem != null)
                {
                    model.ContentItem = new ContentItem {Created = DateTime.Now};
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
                        throw new Exception("The default master template isn't confugured. Please contact support.");
                    }
                }
                var categoryContent = new ProductCategoryContent();
                await UpdateCategory(categoryContent, model);
                return categoryContent;
            }
            else
            {
                var categoryContent = await GetCategoryAsync(model.Id);
                if (categoryContent != null && categoryContent.StatusCode != RecordStatusCode.Deleted)
                {
                    await UpdateCategory(categoryContent, model);
                }
                return categoryContent;
            }
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

	    public async Task<ProductNavCategoryLite> GetLiteCategoriesTreeAsync(ProductCategoryLiteFilter liteFilter)
	    {
		    var productRootCategory = await GetCategoriesTreeAsync(new ProductCategoryTreeFilter() {Statuses = liteFilter.Statuses});

			return await GetLiteCategoriesTreeAsync(productRootCategory, liteFilter);
        }

        public async Task<ProductNavCategoryLite> GetLiteCategoriesTreeAsync(ProductCategory productRootCategory,
            ProductCategoryLiteFilter liteFilter)
        {
            var contentCategoryQuery = new ProductCategoryContentQuery().WithVisibility(liteFilter.Visibility);
            var contentCategories =
                await productCategoryRepository.Query(contentCategoryQuery).SelectAsync(p => new ProductCategoryLite
                {
                    Id = p.Id,
                    NavLabel = p.NavLabel,
                    Url = p.Url
                }, false);

            return new ProductNavCategoryLite
            {
                SubItems =
                    ConvertToTransferCategory(productRootCategory.SubCategories,
                        contentCategories.ToDictionary(c => c.Id))
            };
        }

        #region Private

	    private IList<ProductNavCategoryLite> ConvertToTransferCategory(IEnumerable<ProductCategory> subCategories, IDictionary<int, ProductCategoryLite> contentCategories)
	    {
		    return subCategories.Where(x => contentCategories.ContainsKey(x.Id)).Select(x => new ProductNavCategoryLite
		    {
                Id = x.Id,
                ProductCategory = x,
				NavLabel = contentCategories[x.Id].NavLabel,
				Url = contentCategories[x.Id].Url,
				SubItems = ConvertToTransferCategory(x.SubCategories, contentCategories)
			}).ToList();
	    }

        private ProductNavCategoryLite FindUICategory(ProductNavCategoryLite uiRoot, int id)
        {
            if (uiRoot.Id == id)
            {
                if (uiRoot.ProductCategory.ParentId.HasValue)
                {
                    return uiRoot;
                }
            }
            else
            {
                return
                    uiRoot.SubItems.Select(subCategory => FindUICategory(subCategory, id))
                        .FirstOrDefault(res => res != null);
            }

            return null;
        }

        #endregion
    }
}