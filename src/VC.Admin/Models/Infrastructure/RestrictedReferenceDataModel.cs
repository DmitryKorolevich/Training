using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Models.Infrastructure
{
    public class RestrictedReferenceData
    {
		public IList<LookupItemModel<int>> ValidationMessages { get; set; }
	}
}