using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public class Lookup : Entity
    {
	    public string LookupType { get; set; }

	    public IList<LookupValue> LookupValues { get; set; }
    }
}