using System.Collections.Generic;

namespace VitalChoice.DynamicData.Entities.Transfer
{
    public class SkuOrdered
    {
        public SkuOrdered()
        {
            Messages = new List<string>();
        }

        public SkuDynamic Sku { get; set; }
        public ProductDynamic ProductWithoutSkus { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public IList<string> Messages { get; set; }
    }
}