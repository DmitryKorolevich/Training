namespace VitalChoice.Ecommerce.Domain.Entities.Base
{
    public class LookupVariant : Entity
    {
        public int IdLookup { get; set; }

	    public Lookup Lookup { get; set; }

	    public string ValueVariant { get; set; }

        public int Order { get; set; }

        public bool Hidden { get; set; }
    }
}
