using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Base
{
    public class OptionValueQuery<TOptionType, TOptionValue> : QueryObject<TOptionValue>
        where TOptionValue: OptionValue<TOptionType> 
        where TOptionType : OptionType
    {

    }
}
