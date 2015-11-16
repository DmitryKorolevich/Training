using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class RefundSku : Entity
    {
        public int IdOrder { get; set; }

        public Order Order { get; set; }

        public int IdSku { get; set; }

        public Sku Sku { get; set; }

        public RedeemType Redeem { get; set; }

        public int Quantity { get; set; }

        public decimal RefundValue { get; set; }

        public double RefundPercent { get; set; }

    }
}
