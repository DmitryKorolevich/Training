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
    public class DropShipAAFESSOrderImportProcessor : BaseOrderImportProcessor
    {
        public DropShipAAFESSOrderImportProcessor(
            ICountryService countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
            : base(countryService, orderMapper, addressMapper)
        {
        }

        protected override Type RecordType => typeof (DropShipAAFESShipImportItem);
        protected override Type RecordMapType => typeof(OrderDropShipAAFESSImportItemCsvMap);

        protected override void ParseAdditionalInfo(OrderBaseImportItem baseItem, CsvReader csv, PropertyInfo[] modelProperties, ref List<MessageInfo> messages)
        {
            throw new NotImplementedException();
        }

        protected override Task<List<OrderImportItemOrderDynamic>> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderImportType orderType, CustomerDynamic customer,
            CustomerPaymentMethodDynamic paymentMethod, int idAddedBy)
        {
            throw new NotImplementedException();
        }
    }
}
