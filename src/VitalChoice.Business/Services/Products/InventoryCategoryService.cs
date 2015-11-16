using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.Services.Products
{
    public class InventoryCategoryService : IInventoryCategoryService
    {
        private readonly IEcommerceRepositoryAsync<InventoryCategory> inventoryCategoryEcommerceRepository;
        private readonly IEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue> productService;
        private readonly ILogger logger;

        public InventoryCategoryService(IEcommerceRepositoryAsync<InventoryCategory> inventoryCategoryEcommerceRepository,
            IEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue> productService,
            ILoggerProviderExtended loggerProvider)
        {
            this.inventoryCategoryEcommerceRepository = inventoryCategoryEcommerceRepository;
            this.productService = productService;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<IList<InventoryCategory>> GetCategoriesTreeAsync(InventoryCategoryTreeFilter filter)
        {
            IList<InventoryCategory> toReturn = new List<InventoryCategory>();
            var query = new InventoryCategoryQuery().NotDeleted().WithStatus(filter.Status);
            List<InventoryCategory> categories = await inventoryCategoryEcommerceRepository.Query(query).SelectAsync(false);
            toReturn = categories.Where(p => !p.ParentId.HasValue).OrderBy(p=>p.Order).ToList();

            categories.RemoveAll(p => !p.ParentId.HasValue);
            foreach (var resCategory in toReturn)
            {
                resCategory.CreateSubCategoriesList(categories);
            }

            return toReturn;
        }

        public async Task<bool> UpdateCategoriesTreeAsync(IList<InventoryCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentException();
            }

            var query = new InventoryCategoryQuery().NotDeleted();
            List<InventoryCategory> dbCategories = await inventoryCategoryEcommerceRepository.Query(query).SelectAsync();
            int order = 0;
            foreach (var category in categories)
            {
                category.Order = order;
                order++;
                category.SetSubCategoriesOrder();
            }

            foreach (var dbCategory in dbCategories)
            {
                InventoryCategory uiCategory = FindUICategory(categories, dbCategory.Id);
                if (uiCategory != null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await inventoryCategoryEcommerceRepository.UpdateAsync(dbCategory);
                }
            }

            return true;
        }

        public async Task<InventoryCategory> GetCategoryAsync(int id)
        {
            InventoryCategoryQuery query = new InventoryCategoryQuery().WithId(id).NotDeleted();
            return (await inventoryCategoryEcommerceRepository.Query(query).SelectAsync(false)).FirstOrDefault();
        }

        public async Task<InventoryCategory> UpdateCategoryAsync(InventoryCategory model)
        {
            InventoryCategory dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new InventoryCategory() {ParentId=model.ParentId };
                if (model.ParentId.HasValue)
                {
                    var parentExist = await inventoryCategoryEcommerceRepository.Query(p => p.Id == model.ParentId && p.StatusCode != RecordStatusCode.Deleted)
                        .SelectAnyAsync();
                    if (!parentExist)
                    {
                        throw new AppValidationException("The category with the given parent id doesn't exist.");
                    }

                    var products = await GetProductsAssignedToInvenotyCategory(model.ParentId.Value);
                    if (products.Count > 0)
                    {
                        string message = "Can't add a new category under the category with assigned products. Product names: ";
                        foreach (var product in products)
                        {
                            message += product.Name + ", ";
                        }
                        message=message.Remove(message.Length-3,2);
                        throw new AppValidationException(message);
                    }
                }

                var subCategories = await inventoryCategoryEcommerceRepository.Query(p => p.ParentId == model.ParentId &&
                                                                        p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);
                if (subCategories.Count != 0)
                {
                    dbItem.Order = subCategories.Max(p => p.Order) + 1;
                }

                dbItem.StatusCode = RecordStatusCode.Active;
            }
            else
            {
                dbItem = await GetCategoryAsync(model.Id);
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                dbItem.Name = model.Name;

                if (model.Id == 0)
                {
                    await inventoryCategoryEcommerceRepository.InsertAsync(dbItem);
                }
                else
                {
                    await inventoryCategoryEcommerceRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await inventoryCategoryEcommerceRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                List<MessageInfo> errors = new List<MessageInfo>();
                //Check sub categories
                InventoryCategoryQuery query = new InventoryCategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await inventoryCategoryEcommerceRepository.Query(query).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    errors.Add(new MessageInfo() { Message = "Category with subcategories can't be deleted." });
                }
                
                var products = await GetProductsAssignedToInvenotyCategory(id);
                if (products.Count>0)
                {
                    string message = "Category with products can't be deleted. Product names: ";
                    foreach(var product in products)
                    {
                        message += product.Name+", ";
                    }
                    message=message.Remove(message.Length - 3, 2);
                    errors.Add(new MessageInfo() { Message = message });
                }

                if (errors.Count>0)
                {
                    throw new AppValidationException(errors);
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await inventoryCategoryEcommerceRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #region Private

        private async Task<IList<ProductDynamic>> GetProductsAssignedToInvenotyCategory(int id)
        {
            Dictionary<string, object> filter = new Dictionary<string, object> {{ProductConstants.FIELD_NAME_INVENTORY_CATEGORY_ID, id}};
            var products = await productService.SelectAsync(p => p.WhenValues(filter));
            return products;
        }

        private InventoryCategory FindUICategory(IList<InventoryCategory> categories, int id)
        {
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    if (category.Id == id)
                    {
                        return category;
                    }
                    else
                    {
                        var result = FindUICategory(category.SubCategories, id);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        #endregion
    }
}