using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VC.Public.Models.Product
{
    public class FullReviewsModel
    {
	    public Guid ProductPublicId { get; set; }

	    public string ProductTitle { get; set; }

	    public string ProductSubTitle { get; set; }

	    public string ProductUrl { get; set; }

	    public double AverageRatings { get; set; }

	    public PagedListEx<ReviewListItemModel> Reviews { get; set; }
    }
}
