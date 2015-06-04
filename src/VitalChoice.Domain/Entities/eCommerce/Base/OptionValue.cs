namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class OptionValue<TOptionType> : Entity
        where TOptionType: OptionType
    {
        public int IdOptionType { get; set; }

        public string Value { get; set; }

        public TOptionType OptionType { get; set; }
    }
}
