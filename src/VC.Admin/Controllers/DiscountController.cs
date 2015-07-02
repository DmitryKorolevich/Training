using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System.Security.Claims;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces.Services;
using VitalChoice.Interfaces.Services;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly DiscountMapper _mapper;
        private readonly ILogger _logger;

        public DiscountController(IDiscountService discountService, IProductService productService, ILoggerProviderExtended loggerProvider, DiscountMapper mapper)
        {
            this._discountService = discountService;
            this._productService = productService;
            _mapper = mapper;
            this._logger = loggerProvider.CreateLoggerDefault();
        }

        #region Products
        
        [HttpPost]
        public async Task<Result<PagedList<DiscountListItemModel>>> GetDiscounts([FromBody]DiscountFilter filter)
        {
            var result = await _discountService.GetDiscountsAsync(filter);

            var toReturn = new PagedList<DiscountListItemModel>
            {
                Items = result.Items.Select(p => new DiscountListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<DiscountManageModel>> GetDiscount(int id)
        {
            if (id == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new DiscountManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Assigned = CustomerTypeCode.All,
                    DiscountType=DiscountType.PercentDiscount,
                    StartDate = now,
                    ExpirationDate= now.AddDays(30),
                    DiscountsToSelectedSkus = new List<DiscountToSelectedSku>(),
                    DiscountsToSkus = new List<DiscountToSku>(),
                    DiscountTiers = new List<DiscountTier>(),
                    CategoryIds = new List<int>(),
                };
            }

            var item = await _discountService.GetDiscountAsync(id);
            DiscountManageModel toReturn = _mapper.ToModel<DiscountManageModel>(item);
            int skuId = 0;
            if(item.DictionaryData.ContainsKey("ProductSKU") && item.DictionaryData["ProductSKU"] is string && Int32.TryParse((string)item.DictionaryData["ProductSKU"],out skuId))
            {
                var sku = await _productService.GetSku(skuId);
                if(sku!=null)
                {
                    toReturn.ProductSKU = sku.Code;
                }
            }
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<DiscountManageModel>> UpdateDiscount([FromBody]DiscountManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = _mapper.FromModel(model);
            if (item.DiscountType == DiscountType.Threshold)
            {
                var sku = await _productService.GetSku(model.ProductSKU);
                if (sku == null)
                {
                    throw new AppValidationException("ProductSKU", "The give SKU doesn't exist.");
                }
                item.Data.ProductSKU = sku.Id;
            }

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }

            item = (await _discountService.UpdateDiscountAsync(item));

            return _mapper.ToModel<DiscountManageModel>(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteDiscount(int id)
        {
            return await _discountService.DeleteDiscountAsync(id);
        }

        #endregion
    }
}