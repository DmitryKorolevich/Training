using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Base
{
    public class OptionValueQuery<TOptionType, TOptionValue> : QueryObject<TOptionValue>
        where TOptionValue: OptionValue<TOptionType> 
        where TOptionType : OptionType
    {

    }
}
