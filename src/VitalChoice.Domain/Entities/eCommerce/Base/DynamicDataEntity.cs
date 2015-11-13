using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class DynamicDataEntity : Entity
    {
        public int StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public int? IdEditedBy { get; set; }

        public int IdObjectType { get; set; }

        public User EditedBy { get; set; }
    }

    public abstract class DynamicDataEntity<TOptionValue> : DynamicDataEntity
        where TOptionValue: OptionValue
    {
        public ICollection<TOptionValue> OptionValues { get; set; }
    }

    public abstract class DynamicDataEntity<TOptionValue, TOptionType> : DynamicDataEntity<TOptionValue>
        where TOptionValue: OptionValue<TOptionType>
        where TOptionType : OptionType
    {
        public ICollection<TOptionType> OptionTypes { get; set; }

        //public IEnumerable<TOptionValue> ExceptOptionsIn(
        //    DynamicDataEntity<TOptionValue, TOptionType> updated)
        //{
        //    return OptionValues.ExceptKeyedWith(updated.OptionValues,
        //        item => item.IdOptionType);
        //}

        //public IEnumerable<TOptionValue> IntersectOptionsIn(
        //    DynamicDataEntity<TOptionValue, TOptionType> updated)
        //{
        //    return OptionValues.IntersectKeyedWith(updated.OptionValues,
        //        item => item.IdOptionType);
        //}
    }
}