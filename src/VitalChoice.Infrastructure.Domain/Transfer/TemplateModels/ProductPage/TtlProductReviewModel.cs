using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductReviewModel
    {
	    public string Title { get; set; }

	    public string Review { get; set; }

	    public string CustomerName { get; set; }

	    public DateTime DateCreated { get; set; }

	    public double Rating { get; set; }
    }
}
