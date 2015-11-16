namespace VitalChoice.Ecommerce.Domain.Entities.Base
{
    public abstract class OptionType : Entity
    {
        public string Name { get; set; }

        public int? IdLookup { get; set; }

        public Lookup Lookup { get; set; }

        public int IdFieldType { get; set; }

        public int? IdObjectType { get; set; }

        public string DefaultValue { get; set; }
    }
}
