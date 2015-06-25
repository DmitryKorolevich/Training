namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class OptionType : Entity
    {
        public string Name { get; set; }

        public int? IdLookup { get; set; }

        public Lookup Lookup { get; set; }

        public int IdFieldType { get; set; }

        public string DefaultValue { get; set; }
    }
}
