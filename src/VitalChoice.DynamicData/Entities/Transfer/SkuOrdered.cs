namespace VitalChoice.DynamicData.Entities.Transfer
{
    public class SkuOrdered
    {
        public SkuDynamic Sku { get; set; }
        public ProductDynamic ProductWithoutSkus { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
    }
}