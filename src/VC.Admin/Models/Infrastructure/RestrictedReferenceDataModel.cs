using System.Collections.Generic;
using VitalChoice.Domain.Transfer.Base;

namespace VC.Admin.Models.Infrastructure
{
    public class RestrictedReferenceData
    {
		public IList<LookupItem<string>> Labels { get; set; }
	}
}