using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Business.CsvExportMaps
{
    public class OrderGiftListImportItemCsvMap : CsvClassMap<OrderGiftListImportItem>
    {
        public OrderGiftListImportItemCsvMap()
        {
            Map(m => m.OrderNotes).Name("order notes");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Company).Name("Company");
            Map(m => m.Address).Name("Address 1");
            Map(m => m.Address2).Name("Address 2");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.Country).Name("Country");
            Map(m => m.Zip).Name("Postal Code");
            Map(m => m.Phone).Name("Phone");
            Map(m => m.Email).Name("Email");
            Map(m => m.GiftMessage).Name("Greeting");
        }
    }
}
