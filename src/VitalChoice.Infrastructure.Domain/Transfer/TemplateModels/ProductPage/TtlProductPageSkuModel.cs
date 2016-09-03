using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageSkuModel
    {
        [Map]
	    public string Code { get; set; }

        [Map]
        public string SalesText { get; set; }

        [Map("QTY")]
	    public int PortionsCount { get; set; }

	    public decimal Price { get; set; }

	    public bool BestValue { get; set; }

        public bool InStock { get; set; }

        [Map("AutoShipProduct")]
	    public bool AutoShip { get; set; }
    }
}
