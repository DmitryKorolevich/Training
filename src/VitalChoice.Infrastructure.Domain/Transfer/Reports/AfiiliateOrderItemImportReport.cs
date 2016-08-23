using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class AfiiliateOrderItemImportReport
    {
        public string Id { get; set; }
        public ICollection<AfiiliateOrderItemImportExportModel> Items { get; set; }
    }
}