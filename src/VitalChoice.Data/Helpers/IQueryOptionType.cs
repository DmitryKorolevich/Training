using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Data.Helpers
{
    public interface IQueryOptionType<TOptionType> : IQueryObject<TOptionType>
        where TOptionType : OptionType
    {
        Func<TOptionType, int?, bool> GetFilter();
        IQueryOptionType<TOptionType> WithObjectType(int? objectType);
        IQueryOptionType<TOptionType> WithNames(HashSet<string> names);
    }
}