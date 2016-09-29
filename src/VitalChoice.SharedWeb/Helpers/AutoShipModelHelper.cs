using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;
using VitalChoice.SharedWeb.Models.Orders;

namespace VitalChoice.SharedWeb.Helpers
{
    public class AutoShipModelHelper
    {
	    private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
	    private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
	    private readonly IDynamicMapper<OrderDynamic, Order> _orderMapper;
	    private readonly ReferenceData _referenceData;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public AutoShipModelHelper(IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDynamicMapper<OrderDynamic, Order> orderMapper, ReferenceData referenceData, ICountryNameCodeResolver countryNameCodeResolver)
        {
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _orderMapper = orderMapper;
            _referenceData = referenceData;
            _countryNameCodeResolver = countryNameCodeResolver;
        }

        public async Task<AutoShipHistoryItemModel> PopulateAutoShipItemModel(OrderDynamic orderDynamic)
	    {
			var skuItem = orderDynamic.Skus.FirstOrDefault(p => p.Sku.SafeData.AutoShipFrequency1==true
                || p.Sku.SafeData.AutoShipFrequency2 == true 
                || p.Sku.SafeData.AutoShipFrequency3 == true
                || p.Sku.SafeData.AutoShipFrequency6 == true);
	        if (skuItem == null)
	        {
	            skuItem = orderDynamic.Skus.First();
	        }

			var result = await _skuMapper.ToModelAsync<AutoShipHistoryItemModel>(skuItem.Sku);
			await _productMapper.UpdateModelAsync(result, skuItem.Sku.Product);
            await _orderMapper.UpdateModelAsync(result, orderDynamic);

			var paymentMethod = orderDynamic.PaymentMethod;
			result.PaymentMethodDetails = paymentMethod.PopulateCreditCardDetails(_referenceData);

			var shippingAddress = orderDynamic.ShippingAddress;
			result.ShippingDetails = shippingAddress.PopulateShippingAddressDetails(_countryNameCodeResolver);

			var displayName = result.Name;
			if (!string.IsNullOrWhiteSpace(result.SubTitle))
			{
				displayName += $" {result.SubTitle}";
			}
			displayName += $" ({result.PortionsCount})";

			result.DisplayName = displayName;
			result.Active = orderDynamic.StatusCode == (int)RecordStatusCode.Active;
	        if (orderDynamic.SafeData.LastAutoShipDate != null)
	        {
	            result.NextDate = orderDynamic.Data.LastAutoShipDate.AddMonths(result.Frequency);
	        }
	        else
	        {
	            if (orderDynamic.SafeData.ShipDelayDate != null && orderDynamic.OrderStatus == OrderStatus.ShipDelayed)
	            {
	                result.NextDate = orderDynamic.Data.ShipDelayDate;
	            }
	        }
	        result.Id = orderDynamic.Id;

		    return result;
	    }
    }
}
