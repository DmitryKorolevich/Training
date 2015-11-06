using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Data.Helpers
{
    public interface IQueryOptionType<TOptionType> : IQueryObject<TOptionType>
        where TOptionType : OptionType
    {
        IQueryOptionType<TOptionType> WithObjectType(int? objectType);
        IQueryOptionType<TOptionType> WithNames(HashSet<string> names);
    }
}