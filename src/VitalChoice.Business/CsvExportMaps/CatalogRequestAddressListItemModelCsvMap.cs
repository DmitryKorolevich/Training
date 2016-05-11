using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests;

namespace VitalChoice.Business.CsvExportMaps
{
    public class CatalogRequestAddressListItemModelCsvMap : CsvClassMap<CatalogRequestAddressListItemModel>
    {
        public CatalogRequestAddressListItemModelCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.PersonTitle).Name("Title").Index(0);
            Map(m => m.FirstName).Name("First Name").Index(1);
            Map(m => m.LastName).Name("Last Name").Index(2);
            Map(m => m.Address1).Name("Address").Index(3);
            Map(m => m.City).Name("City").Index(4);
            Map(m => m.Country).Name("Country").Index(5);
            Map(m => m.StateCode).Name("State").Index(6);
            Map(m => m.County).Name("County").Index(7);
            Map(m => m.Zip).Name("Postal Code").Index(8);
            Map(m => m.Email).Name("Email").Index(9);
        }
    }
}
