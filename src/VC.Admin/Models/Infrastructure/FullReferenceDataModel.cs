using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Models.Infrastructure
{
    public class FullReferenceDataModel : RestrictedReferenceData
    {
		public IList<LookupItemModel<string>> Roles { get; set; }
    }
}