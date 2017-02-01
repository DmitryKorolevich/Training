using System;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Business.CsvExportMaps.Orders;

namespace VitalChoice.Business.CsvExportMaps
{
    public sealed class AffiliateListItemModelCsvMap : CsvClassMap<AffiliateListItemModel>
    {
        public AffiliateListItemModelCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.Company).Name("Company");
            Map(m => m.WebSite).Name("Web Site");
            Map(m => m.CommissionDescription).Name("Commission");
            Map(m => m.StatusCode).Name("Status").TypeConverter<AffiliateStatusCodeConverter>();
            Map(m => m.Tier).Name("Tier");
            Map(m => m.CustomersCount).Name("Customers");
            Map(m => m.NotPaidCommissionsAmount).Name("Commission Balance*").TypeConverterOption("c");
            Map(m => m.PaymentType).Name("Payment as").TypeConverter<AffiliatePaymentTypeConverter>();
            Map(m => m.DateEdited).Name("Last Updated").TypeConverterOption(CultureInfo.InvariantCulture)
                .TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.EditedByAgentId).Name("Last Updated by");
            Map(m => m.Email).Name("Email");
        }
    }
}
