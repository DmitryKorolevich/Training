using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Base
{
    public class OptionTypeQuery<TOptionType> : QueryObject<TOptionType>, IQueryOptionType<TOptionType> 
        where TOptionType : OptionType
    {
        public virtual IQueryOptionType<TOptionType> WithObjectType(int? objectType)
        {
            Add(t => t.IdObjectType == objectType || objectType != null && t.IdObjectType == null);
            return this;
        }

        public virtual IQueryOptionType<TOptionType> WithNames(HashSet<string> names)
        {
            Add(t => names.Contains(t.Name));
            return this;
        }
    }
}