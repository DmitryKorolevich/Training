using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class SkuGoogleItemExportCsvMap : CsvClassMap<SkuGoogleItem>
    {
        public SkuGoogleItemExportCsvMap()
        {
            Map(m => m.Id).Name("id").Index(0);
            Map(m => m.Title).Name("title").Index(1);
            Map(m => m.Url).Name("link").Index(2);
            Map(m => m.RetailPrice).Name("price").Index(3).TypeConverterOption("N2");
            Map(m => m.Description).Name("description").Index(4);
            Map(m => m.Condition).Name("condition").Index(5);
            Map(m => m.Brand).Name("brand").Index(6);
            Map(m => m.SkuCode).Name("mpn").Index(7);
            Map(m => m.Thumbnail).Name("image_link").Index(8);
            Map(m => m.GoogleCategory).Name("google_product_category").Index(9);
            Map(m => m.ProductRootCategory).Name("product_type").Index(10);
            Map(m => m.Availability).Name("availability").Index(11);
            Map(m => m.SkuCodeGroup).Name("item_group_id").Index(12);
            Map(m => m.MainProductImage).Name("additional_image_link").Index(13);
            Map(m => m.Quantity).Name("quantity").Index(14);
            Map(m => m.Manufacturer).Name("manufacturer").Index(15);
            Map(m => m.Seller).Name("custom_label_1").Index(16);
        }
    }
}
