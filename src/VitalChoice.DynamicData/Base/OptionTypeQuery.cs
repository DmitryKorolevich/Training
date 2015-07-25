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
            Add(t => t.IdObjectType == objectType);
            if (objectType.HasValue)
            {
                Or(t => t.IdObjectType == null);
            }
            return this;
        }

        public virtual IQueryOptionType<TOptionType> WithNames(ICollection<string> names)
        {
            Add(t => names.Contains(t.Name));
            return this;
        }
    }
}