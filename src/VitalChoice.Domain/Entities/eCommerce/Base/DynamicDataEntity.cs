using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class DynamicDataEntity<TOptionValue, TOptionType> : Entity
        where TOptionValue: OptionValue<TOptionType>
        where TOptionType : OptionType
    {
        public ICollection<TOptionValue> OptionValues { get; set; }

        public ICollection<TOptionType> OptionTypes { get; set; }
    }
}