using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Services.Orders
{
    public abstract class StandartOrderImportProcessor : BaseOrderImportProcessor
    {
        protected StandartOrderImportProcessor(
            ICountryService countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ReferenceData referenceData)
            : base(countryService, orderMapper, addressMapper, referenceData)
        {
        }

        protected ICollection<OrderSkuImportItem> ParseOrderSkus(CsvReader reader, string skuColumnName, string qtyColumnName, ref List<MessageInfo> messages)
        {
            List<OrderSkuImportItem> toReturn = new List<OrderSkuImportItem>();
            int number = 1;
            bool existInHeaders = true;
            while (existInHeaders)
            {
                existInHeaders = reader.FieldHeaders.Contains($"{skuColumnName} {number}");
                existInHeaders = existInHeaders && reader.FieldHeaders.Contains($"{qtyColumnName} {number}");

                if (existInHeaders)
                {
                    var sku = reader.GetField<string>($"{skuColumnName} {number}");
                    var sqty = reader.GetField<string>($"{qtyColumnName} {number}");

                    if (!String.IsNullOrEmpty(sku) || !String.IsNullOrEmpty(sqty))
                    {
                        int qty = 0;
                        Int32.TryParse(sqty, out qty);
                        if (qty == 0)
                        {
                            messages.Add(AddErrorMessage($"{qtyColumnName} {number}", String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseIntError], $"{qtyColumnName} {number}")));
                        }
                        else
                        {
                            OrderSkuImportItem item = new OrderSkuImportItem();
                            item.SKU = sku;
                            item.QTY = qty;
                            toReturn.Add(item);
                        }
                    }
                }

                number++;
            }
            if (toReturn.Count == 0)
            {
                messages.Add(AddErrorMessage(null, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ZeroSkusForOrderInImport])));
            }
            return toReturn;
        }
    }
}
