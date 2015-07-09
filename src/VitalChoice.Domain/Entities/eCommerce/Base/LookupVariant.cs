namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public class LookupVariant : Entity
    {
        public int IdLookup { get; set; }

	    public Lookup Lookup { get; set; }

	    public string ValueVariant { get; set; }
    }
}
