using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Models.Infrastructure
{
    public class RestrictedReferenceData
    {
		public IList<LookupItem<string>> Labels { get; set; }
	}
}