using System.Collections.Generic;

namespace VitalChoice.Domain.Transfer.Base
{
    public class ReferenceData
    {
	    public IList<LookupItem<string>> Roles { get; set; }

		public IList<LookupItem<byte>> UserStatuses { get; set; }
	}
}