﻿using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Transfer.Base
{
    public class ReferenceData
    {
	    public IList<LookupItem<int>> Roles { get; set; }

		public IList<LookupItem<byte>> UserStatuses { get; set; }
        
        public IList<LookupItem<int>> ContentTypes { get; set; }

        public IList<ContentProcessor> ContentProcessors { get; set; }

        public IList<LookupItem<string>> Labels { get; set; }

        public string PublicHost { get; set; }

        public IList<LookupItem<string>> ContentItemStatusNames { get; set; }

        public IList<LookupItem<string>> ProductCategoryStatusNames { get; set; }

        public IList<LookupItem<int>> GCTypes { get; set; }

        public IList<LookupItem<int>> RecordStatuses { get; set; }

        public IList<LookupItem<int>> ProductTypes { get; set; }
    }
}