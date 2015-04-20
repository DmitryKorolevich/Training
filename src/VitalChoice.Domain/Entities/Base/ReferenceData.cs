using System.Collections;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Base
{
    public class ReferenceData
    {
	    public IList<LookupItem<string>> Roles { get; set; }
    }
}