using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using System.Text;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using Microsoft.Extensions.OptionsModel;
using VC.Admin.Models.InventorySkus;
using VC.Admin.Models.Products;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;

namespace VC.Admin.Controllers
{
    public class InventorySkuController : BaseApiController
    {
        private readonly IInventorySkuCategoryService _inventorySkuCategoryService;
        private readonly IInventorySkuService _inventorySkuService;
        private readonly InventorySkuMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        public InventorySkuController(
            IInventorySkuCategoryService inventorySkuCategoryService,
            IInventorySkuService inventorySkuService,
            InventorySkuMapper mapper,
            ISettingService settingService,
            ILoggerProviderExtended loggerProvider)
        {
            _inventorySkuCategoryService = inventorySkuCategoryService;
            _inventorySkuService = inventorySkuService;
            _mapper = mapper;
            _settingService = settingService;
            _logger = loggerProvider.CreateLoggerDefault();
        }


        [HttpGet]
        public async Task<Result<IList<Lookup>>> GetInventorySkuLookups()
        {
            var result = await _settingService.GetLookupsAsync(SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(','));
            result.ForEach(p =>
            {
                if (p.LookupVariants != null)
                {
                    foreach (var lookupVariant in p.LookupVariants)
                    {
                        lookupVariant.Lookup = null;
                    }
                }
            });

            return result.ToList();
        }

        #region InventorySkus

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<PagedList<InventorySkuListItemModel>>> GetInventorySkus([FromBody]InventorySkuFilter filter)
        {
            var toReturn = await _inventorySkuService.GetInventorySkusAsync(filter);

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuManageModel>> GetInventorySku(int id)
        {
            if (id == 0)
            {
                return new InventorySkuManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                };
            }

            var item = await _inventorySkuService.SelectAsync(id);
            if (item == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }
            InventorySkuManageModel toReturn = _mapper.ToModel<InventorySkuManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuManageModel>> UpdateInventorySku([FromBody]InventorySkuManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = _mapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            if (item.Id > 0)
            {
                item = await _inventorySkuService.UpdateAsync(item);
            }
            else
            {
                item = await _inventorySkuService.InsertAsync(item);
            }

            var toReturn = _mapper.ToModel<InventorySkuManageModel> (item);

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> DeleteInventorySku(int id, [FromBody] object model)
        {
            return await _inventorySkuService.DeleteAsync(id);
        }

        #endregion

        #region InventorySkuCategories

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<IList<InventorySkuCategoryTreeItemModel>>> GetInventorySkuCategoriesTree([FromBody]InventorySkuCategoryTreeFilter filter)
        {
            var result = await _inventorySkuCategoryService.GetCategoriesTreeAsync(filter);

            return result.Select(p => new InventorySkuCategoryTreeItemModel(p)).ToList();
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> UpdateInventorySkuCategoriesTree([FromBody]IList<InventorySkuCategoryTreeItemModel> model)
        {
            IList<InventorySkuCategory> categories = new List<InventorySkuCategory>();
            if (categories != null)
            {
                foreach (var modelCategory in model)
                {
                    categories.Add(modelCategory.Convert());
                }
            }

            return await _inventorySkuCategoryService.UpdateCategoriesTreeAsync(categories);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuCategoryManageModel>> GetInventorySkuCategory(int id)
        {
            if (id == 0)
            {
                return new InventorySkuCategoryManageModel()
                {
                };
            }
            return new InventorySkuCategoryManageModel((await _inventorySkuCategoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuCategoryManageModel>> UpdateInventorySkuCategory([FromBody]InventorySkuCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await _inventorySkuCategoryService.UpdateCategoryAsync(item);

            return new InventorySkuCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> DeleteInventorySkuCategory(int id, [FromBody] object model)
        {
            return await _inventorySkuCategoryService.DeleteCategoryAsync(id);
        }

        #endregion

    }
}