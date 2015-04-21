using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Models.Infrastructure
{
    public class RestrictedReferenceData
    {
		public IList<LookupItem<int>> ValidationMessages { get; set; }
	}
}