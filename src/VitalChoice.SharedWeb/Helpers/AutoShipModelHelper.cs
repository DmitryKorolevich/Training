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
using VitalChoice.SharedWeb.Models.Orders;

namespace VitalChoice.SharedWeb.Helpers
{
    public class AutoShipModelHelper
    {
	    private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
	    private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
	    private readonly IDynamicMapper<OrderDynamic, Order> _orderMapper;
	    private readonly ReferenceData _referenceData;
	    private readonly ICollection<Country> _countries;

	    public AutoShipModelHelper(IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper, IDynamicMapper<OrderDynamic, Order> orderMapper, ReferenceData referenceData, ICollection<Country> countries)
	    {
		    _skuMapper = skuMapper;
		    _productMapper = productMapper;
		    _orderMapper = orderMapper;
		    _referenceData = referenceData;
		    _countries = countries;
	    }

	    public AutoShipHistoryItemModel PopulateAutoShipItemModel(OrderDynamic orderDynamic)
	    {
			var skuItem = orderDynamic.Skus.First();

			var result = _skuMapper.ToModel<AutoShipHistoryItemModel>(skuItem.Sku);
			_productMapper.UpdateModel(result, skuItem.Sku.Product);
			_orderMapper.UpdateModel(result, orderDynamic);

			var paymentMethod = orderDynamic.PaymentMethod;
			result.PaymentMethodDetails = paymentMethod.PopulateCreditCardDetails(_referenceData);

			var shippingAddress = orderDynamic.ShippingAddress;
			result.ShippingDetails = shippingAddress.PopulateShippingAddressDetails(_countries);

			var displayName = result.Name;
			if (!string.IsNullOrWhiteSpace(result.SubTitle))
			{
				displayName += $" {result.SubTitle}";
			}
			displayName += $" ({result.PortionsCount})";

			result.DisplayName = displayName;
			result.Active = orderDynamic.StatusCode == (int)RecordStatusCode.Active;
	        if (orderDynamic.Data.LastAutoShipDate != null)
	        {
	            result.NextDate = orderDynamic.Data.LastAutoShipDate.AddMonths(result.Frequency);
	        }
	        result.Id = orderDynamic.Id;

		    return result;
	    }
    }
}
