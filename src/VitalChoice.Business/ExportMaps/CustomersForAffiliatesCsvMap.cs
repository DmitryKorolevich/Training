using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Domain.Transfer.Customers;

namespace VitalChoice.Business.ExportMaps
{
    public class CustomersForAffiliatesCsvMap : CsvClassMap<ExtendedVCustomer>
    {
        public CustomersForAffiliatesCsvMap()
        {
            Map(m => m.Id).Name("Customer Id").Index(0);
            Map(m => m.Email).Name("Email").Index(1);
            Map(m => m.FirstName).Name("FirstName").Index(2);
            Map(m => m.LastName).Name("LastName").Index(3);
            Map(m => m.FirstOrderPlaced).Name("First Order").Index(4).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
        }
    }
}
