using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Business.CsvExportMaps
{
    public class VitalGreenRequestCsvMap : CsvClassMap<VitalGreenRequest>
    {
        public VitalGreenRequestCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.FirstName).Name("First Name").Index(0);
            Map(m => m.LastName).Name("Last Name").Index(1);
            Map(m => m.Address).Name("Address").Index(2);
            Map(m => m.City).Name("City").Index(3);
            Map(m => m.State).Name("State").Index(4);
            Map(m => m.Zip).Name("Zip").Index(5);
        }
    }
}
