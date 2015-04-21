using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Models.Infrastructure
{
    public class FullReferenceDataModel : RestrictedReferenceData
    {
		public IList<LookupItem<string>> Roles { get; set; }
	    public IList<LookupItem<int>> UserStatuses { get; set; }
    }
}