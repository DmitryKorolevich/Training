using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class OrdersSummarySalesOrderTypeStatisticItem : Entity
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public decimal Total { get; set; }

        public decimal Average { get; set; }
    }
}
