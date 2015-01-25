namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public class LookupValue : Entity
    {
	    public int LookupId { get; set; }

		public Lookup Lookup { get; set; }

	    public string Value { get; set; }
    }
}