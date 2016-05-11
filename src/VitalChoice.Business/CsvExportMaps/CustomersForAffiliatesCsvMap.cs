using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.CsvExportMaps
{
    public class CustomersForAffiliatesCsvMap : CsvClassMap<ExtendedVCustomer>
    {
        public CustomersForAffiliatesCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Customer Id").Index(0);
            Map(m => m.Email).Name("Email").Index(1);
            Map(m => m.FirstName).Name("FirstName").Index(2);
            Map(m => m.LastName).Name("LastName").Index(3);
            Map(m => m.FirstOrderPlaced).Name("First Order").Index(4).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
        }
    }
}
