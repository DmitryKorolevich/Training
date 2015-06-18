using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;

namespace VC.Admin.Models.Infrastructure
{
    public class FullReferenceDataModel : RestrictedReferenceData
    {
		public IList<LookupItem<int>> Roles { get; set; }
	    public IList<LookupItem<byte>> UserStatuses { get; set; }
        public IList<LookupItem<int>> ContentTypes { get; set; }
        public IList<ContentProcessor> ContentProcessors { get; set; }
        public string PublicHost { get; set; }        
        public IList<LookupItem<string>> ContentItemStatusNames { get; set; }
        public IList<LookupItem<string>> ProductCategoryStatusNames { get; set; }
        public IList<LookupItem<int>> GCTypes { get; set; }
        public IList<LookupItem<int>> RecordStatuses { get; set; }
        public IList<LookupItem<int>> ProductTypes { get; set; }
        public IList<LookupItem<int>> DiscountTypes { get; set; }
        public IList<LookupItem<int>> AssignedCustomerTypes { get; set; }
        public IList<LookupItem<int?>> ActiveFilterOptions { get; set; }
        public IList<LookupItem<int>> CustomerTypes { get; set; }
    }
}