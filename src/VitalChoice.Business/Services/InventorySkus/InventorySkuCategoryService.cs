using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.InventorySkus;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;

namespace VitalChoice.Business.Services.InventorySkus
{
    public class InventorySkuCategoryService : IInventorySkuCategoryService
    {
        private readonly IEcommerceRepositoryAsync<InventorySkuCategory> _inventorySkuCategoryEcommerceRepository;
        private readonly IEcommerceRepositoryAsync<InventorySku> _inventorySkuEcommerceRepository;
        private readonly ILogger _logger;

        public InventorySkuCategoryService(IEcommerceRepositoryAsync<InventorySkuCategory> inventorySkuCategoryEcommerceRepository,
            IEcommerceRepositoryAsync<InventorySku> inventorySkuEcommerceRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _inventorySkuCategoryEcommerceRepository = inventorySkuCategoryEcommerceRepository;
            _inventorySkuEcommerceRepository = inventorySkuEcommerceRepository;
            _logger = loggerProvider.CreateLogger<InventorySkuCategoryService>();
        }

        public async Task<IList<InventorySkuCategory>> GetCategoriesTreeAsync(InventorySkuCategoryTreeFilter filter)
        {
            var query = new InventorySkuCategoryQuery().NotDeleted().WithStatus(filter.Status);
            List<InventorySkuCategory> categories = await _inventorySkuCategoryEcommerceRepository.Query(query).SelectAsync(false);
            IList<InventorySkuCategory> toReturn = categories.Where(p => !p.ParentId.HasValue).OrderBy(p=>p.Order).ToList();

            categories.RemoveAll(p => !p.ParentId.HasValue);
            foreach (var resCategory in toReturn)
            {
                resCategory.CreateSubCategoriesList(categories);
            }

            return toReturn;
        }

        public async Task<bool> UpdateCategoriesTreeAsync(IList<InventorySkuCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentException();
            }

            var query = new InventorySkuCategoryQuery().NotDeleted();
            List<InventorySkuCategory> dbCategories = await _inventorySkuCategoryEcommerceRepository.Query(query).SelectAsync(true);
            int order = 0;
            foreach (var category in categories)
            {
                category.Order = order;
                order++;
                category.SetSubCategoriesOrder();
            }

            foreach (var dbCategory in dbCategories)
            {
                InventorySkuCategory uiCategory = FindUICategory(categories, dbCategory.Id);
                if (uiCategory != null)
                {
                    dbCategory.ParentId = uiCategory.ParentId;
                    dbCategory.Order = uiCategory.Order;
                    await _inventorySkuCategoryEcommerceRepository.UpdateAsync(dbCategory);
                }
            }

            return true;
        }

        public async Task<InventorySkuCategory> GetCategoryAsync(int id)
        {
            InventorySkuCategoryQuery query = new InventorySkuCategoryQuery().WithId(id).NotDeleted();
            return (await _inventorySkuCategoryEcommerceRepository.Query(query).SelectAsync(false)).FirstOrDefault();
        }

        public async Task<InventorySkuCategory> UpdateCategoryAsync(InventorySkuCategory model)
        {
            InventorySkuCategory dbItem;
            if (model.Id == 0)
            {
                dbItem = new InventorySkuCategory() {ParentId=model.ParentId };
                if (model.ParentId.HasValue)
                {
                    var parentExist = await _inventorySkuCategoryEcommerceRepository.Query(p => p.Id == model.ParentId && p.StatusCode != RecordStatusCode.Deleted)
                        .SelectAnyAsync();
                    if (!parentExist)
                    {
                        throw new AppValidationException("The category with the given parent id doesn't exist.");
                    }

                    var items = await GetInventorySkusAssignedToInvenotySkuCategory(model.ParentId.Value);
                    if (items.Count > 0)
                    {
                        string message = "Can't add a new category under the category with assigned inventory SKUs. SKU codes: ";
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];
                            if (i != items.Count - 1)
                            {
                                message += item.Code + ", ";
                            }
                        }
                        throw new AppValidationException(message);
                    }
                }

                var subCategories = await _inventorySkuCategoryEcommerceRepository.Query(p => p.ParentId == model.ParentId &&
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
                    await _inventorySkuCategoryEcommerceRepository.InsertAsync(dbItem);
                }
                else
                {
                    await _inventorySkuCategoryEcommerceRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _inventorySkuCategoryEcommerceRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                List<MessageInfo> errors = new List<MessageInfo>();
                //Check sub categories
                InventorySkuCategoryQuery query = new InventorySkuCategoryQuery().WithParentId(id).NotDeleted();
                var subCategoriesExist = (await _inventorySkuCategoryEcommerceRepository.Query(query).SelectAnyAsync());
                if (subCategoriesExist)
                {
                    errors.Add(new MessageInfo() { Message = "Category with subcategories can't be deleted." });
                }
                var items = await GetInventorySkusAssignedToInvenotySkuCategory(id);
                if (items.Count > 0)
                {
                    string message = "Category with inventory codes can't be deleted: Codes: ";
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        message += item.Code;
                        if (i != items.Count - 1)
                        {
                            message += ", ";
                        }
                    }
                    throw new AppValidationException(message);
                }

                if (errors.Count>0)
                {
                    throw new AppValidationException(errors);
                }

                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _inventorySkuCategoryEcommerceRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #region Private

        private async Task<IList<InventorySku>> GetInventorySkusAssignedToInvenotySkuCategory(int id)
        {
            var items = await _inventorySkuEcommerceRepository.Query(p=>p.StatusCode!=(int)RecordStatusCode.Deleted && p.IdInventorySkuCategory==id).SelectAsync(false);
            return items;
        }

        private InventorySkuCategory FindUICategory(IList<InventorySkuCategory> categories, int id)
        {
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    if (category.Id == id)
                    {
                        return category;
                    }
                    var result = FindUICategory(category.SubCategories, id);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}