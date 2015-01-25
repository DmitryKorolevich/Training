namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class OptionValue : Entity
    {
	    public string Value { get; set; }

	    public Lookup Lookup { get; set; }

	    public int LookupId { get; set; }

	    public OptionType OptionType { get; set; }

		public int OptionTypeId { get; set; }
    }
}