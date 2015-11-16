namespace VitalChoice.Ecommerce.Domain.Entities.Base
{
    public abstract class OptionValue : Entity
    {
        public int IdOptionType { get; set; }

        public string Value { get; set; }

        public long? IdBigString { get; set; }

        public BigStringValue BigValue { get; set; }
    }

    public abstract class OptionValue<TOptionType> : OptionValue
        where TOptionType : OptionType
    {
        public TOptionType OptionType { get; set; }
    }
}