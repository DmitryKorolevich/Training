using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class DropShipOrderImportProcessor : StandartOrderImportProcessor
    {
        public DropShipOrderImportProcessor(
            ICountryService countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
            : base(countryService, orderMapper, addressMapper)
        {
        }

        protected override Type RecordType => typeof (OrderDropShipImportItem);
        protected override Type RecordMapType => typeof(OrderDropShipImportItemCsvMap);

        protected override void ParseAdditionalInfo(OrderBaseImportItem baseItem, CsvReader csv, PropertyInfo[] modelProperties, ref List<MessageInfo> messages)
        {
            var item = (OrderDropShipImportItem) baseItem;

            var shipDateProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(OrderDropShipImportItem.ShipDelayDate));
            var shipDateHeader = shipDateProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;

            var skuProperties = typeof(OrderSkuImportItem).GetProperties();
            var skuProperty = skuProperties.FirstOrDefault(p => p.Name == nameof(OrderSkuImportItem.SKU));
            var skuBaseHeader = skuProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;
            var qtyProperty = skuProperties.FirstOrDefault(p => p.Name == nameof(OrderSkuImportItem.QTY));
            var qtyBaseHeader = qtyProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name; // ReSharper disable all

            item.ShipDelayDate = ParseOrderShipDate(csv, shipDateHeader, ref messages);
            item.Skus = ParseOrderSkus(csv, skuBaseHeader, qtyBaseHeader, ref messages);

            int? idState = null;
            int? idCountry = null;
            ParseContryAndStateCodes(csv,nameof(OrderDropShipImportItem.State), nameof(OrderDropShipImportItem.Country), ref messages, out idState, out idCountry);
            item.IdState = idState;
            item.IdCountry = idCountry;

            if (!String.IsNullOrEmpty(item.Phone))
            {
                item.Phone = item.Phone.Replace(" ", "").Replace("+", "").Replace("-", "");
            }
        }

        protected override async Task<List<OrderImportItemOrderDynamic>> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderImportType orderType,
            CustomerDynamic customer, CustomerPaymentMethodDynamic paymentMethod, int idAddedBy)
        {
            List<OrderImportItemOrderDynamic> toReturn = new List<OrderImportItemOrderDynamic>();
            foreach (var record in records.Select(p=>(OrderDropShipImportItem)p))
            {
                var order = OrderMapper.CreatePrototype((int)orderType);
                order.IdEditedBy = idAddedBy;
                order.Customer = customer;
                order.ShippingAddress = await AddressMapper.FromModelAsync(record, (int)AddressType.Shipping);
                record.SetFields(order, paymentMethod);
                var item = new OrderImportItemOrderDynamic();
                item.Order = order;
                item.OrderImportItems.Add(record);
                toReturn.Add(item);
            }
            return toReturn;
        }
    }
}
