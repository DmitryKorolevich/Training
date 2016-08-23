using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class AfiiliateOrderItemImportExportModel
    {
        public int IdOrder { get; set; }
        public string StatusMessage { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public OrderStatus? POrderStatus { get; set; }
        public OrderStatus? NPOrderStatus { get; set; }
    }
}