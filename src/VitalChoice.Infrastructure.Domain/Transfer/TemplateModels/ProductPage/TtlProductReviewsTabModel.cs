using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductReviewsTabModel
    {
	    public int ReviewsCount { get; set; }

	    public int AverageRatings { get; set; }

		public IList<TtlProductReviewModel> Reviews { get; set; } 
    }
}
