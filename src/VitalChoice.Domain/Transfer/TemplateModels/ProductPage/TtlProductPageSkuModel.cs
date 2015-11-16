using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageSkuModel
    {
	    public string Code { get; set; }

	    public int PortionsCount { get; set; }

	    public decimal Price { get; set; }
    }
}
