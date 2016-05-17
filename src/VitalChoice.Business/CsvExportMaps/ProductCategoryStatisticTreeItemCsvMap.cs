using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.CsvExportMaps
{
    public class ProductCategoryStatisticTreeItemCsvMap : CsvClassMap<ProductCategoryStatisticTreeItemModel>
    {
        public ProductCategoryStatisticTreeItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Id").Index(0);
            Map(m => m.Name).Name("Name").Index(1);
            Map(m => m.Amount).Name("Tota").Index(2).TypeConverterOption("c");
            Map(m => m.Percent).Name("Percent").Index(3).TypeConverterOption("p");
        }
    }
}
