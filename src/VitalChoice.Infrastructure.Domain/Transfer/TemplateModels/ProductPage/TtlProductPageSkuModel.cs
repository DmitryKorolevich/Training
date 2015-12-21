namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageSkuModel
    {
	    public string Code { get; set; }

	    public string SalesText { get; set; }

	    public int PortionsCount { get; set; }

	    public decimal Price { get; set; }

	    public bool BestValue { get; set; }

        public bool InStock { get; set; }
    }
}
