using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Models.Infrastructure
{
    public class FullReferenceDataModel : RestrictedReferenceData
    {
		public IList<LookupItem<int>> Roles { get; set; }
	    public IList<LookupItem<byte>> UserStatuses { get; set; }
        public IList<LookupItem<int>> ContentTypes { get; set; }
        public IList<ContentProcessor> ContentProcessors { get; set; }
        public IList<LookupItem<string>> Labels { get; set; }
        public string PublicHost { get; set; }        
        public IList<LookupItem<string>> ContentItemStatusNames { get; set; }
        public IList<LookupItem<string>> ProductCategoryStatusNames { get; set; }
    }
}