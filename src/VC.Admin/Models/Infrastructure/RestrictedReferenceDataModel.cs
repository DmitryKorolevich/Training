using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VC.Admin.Models.Infrastructure
{
    public class RestrictedReferenceData
    {
		public IList<LookupItem<string>> Labels { get; set; }

	    public IList<LookupItem<int>> CartShippingOptions { get; set; }
    }
}