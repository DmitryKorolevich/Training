using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Orders
{
    public class DropShipAAFESSOrderImportProcessor : BaseOrderImportProcessor
    {
        private const string SHIPTO_STATE = "SHIPTO_STATE";
        private const string SHIPTO_COUNTRY = "SHIPTO_COUNTRY";
        private const string USA = "United States";

        public DropShipAAFESSOrderImportProcessor(
            ICountryNameCodeResolver countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ReferenceData referenceData, ILoggerFactory loggerFactory)
            : base(countryService, orderMapper, addressMapper, referenceData, loggerFactory.CreateLogger<DropShipAAFESSOrderImportProcessor>())
        {
        }

        protected override Type RecordType => typeof (DropShipAAFESShipImportItem);
        protected override Type RecordMapType => typeof(OrderDropShipAAFESSImportItemCsvMap);

        protected override void ParseAdditionalInfo(OrderBaseImportItem baseItem, CsvReader csv, PropertyInfo[] modelProperties, ref List<MessageInfo> messages)
        {
            var item = (DropShipAAFESShipImportItem)baseItem;

            //do not parse ship data - Gary's request
            //var shipDateProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(DropShipAAFESShipImportItem.ShipDelayDate));
            //var shipDateHeader = shipDateProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;

            //item.ShipDelayDate = ParseOrderShipDate(csv, shipDateHeader, ref messages);

            var stateCode = csv.GetField<string>(SHIPTO_STATE);
            var countryName = csv.GetField<string>(SHIPTO_COUNTRY);
            if (!String.IsNullOrEmpty(countryName))
            {
                var country = CountryService.GetCountryByName(countryName);
                if (country != null)
                {
                    item.IdCountry = country.Id;
                    if (countryName != USA)
                    {
                        messages.Add(AddErrorMessage(SHIPTO_COUNTRY, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], SHIPTO_COUNTRY)));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(stateCode))
                        {
                            var state = CountryService.GetStateByCode(country.Id, stateCode);
                            if (state == null)
                            {
                                messages.Add(AddErrorMessage(SHIPTO_STATE, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], SHIPTO_STATE)));
                            }
                            else
                            {
                                item.IdState = state.Id;
                            }
                        }
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(SHIPTO_COUNTRY, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], SHIPTO_COUNTRY)));
                }
            }

            if (!String.IsNullOrEmpty(item.Phone))
            {
                item.Phone = item.Phone.Replace(" ", "").Replace("+", "").Replace("-", "");
            }
        }

        protected override async Task<List<OrderImportItemOrderDynamic>> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderImportType orderType, CustomerDynamic customer,
            CustomerPaymentMethodDynamic paymentMethod, int idAddedBy)
        {
            Dictionary<string, OrderImportItemOrderDynamic> map =new Dictionary<string, OrderImportItemOrderDynamic>();
            foreach (var record in records.Select(p => (DropShipAAFESShipImportItem)p))
            {
                OrderImportItemOrderDynamic item = null;
                if(!map.TryGetValue(record.OrderNotes, out item))
                {
                    var order = OrderMapper.CreatePrototype((int) OrderType.DropShip);
                    order.IdEditedBy = idAddedBy;
                    order.Customer = customer;
                    var names = record.Company.Split(' ');
                    if (names.Length >= 1)
                    {
                        record.FirstName = names[0];
                    }
                    if (names.Length >= 2)
                    {
                        record.LastName = String.Concat(names.Where((p,i)=>i>0).Select(p=>p+" "));
                        record.LastName = record.LastName.Trim();
                    }
                    if (string.IsNullOrEmpty(record.LastName))
                    {
                        record.LastName = "-";
                    }
                    record.Company = null;
                    order.ShippingAddress = await AddressMapper.FromModelAsync(record, (int) AddressType.Shipping);
                    order.ShippingAddress.Data.PreferredShipMethod = (int) PreferredShipMethod.FedEx;
                    record.SetFields(order, paymentMethod);
                    item = new OrderImportItemOrderDynamic();
                    item.Order = order;
                    map.Add(record.OrderNotes, item);
                }

                item.Order.Skus.Add(new SkuOrdered()
                {
                    Sku = new SkuDynamic() { Code = record.Sku },
                    Quantity = record.QTY,
                });
                item.OrderImportItems.Add(record);
            }
            return map.Select(p=>p.Value).ToList();
        }
    }
}
