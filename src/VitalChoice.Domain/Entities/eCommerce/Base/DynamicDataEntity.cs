using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class DynamicDataEntity<TOptionValue, TOptionType> : Entity
        where TOptionValue: OptionValue<TOptionType>
        where TOptionType : OptionType
    {
        public int StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

		public int? IdEditedBy { get; set; }

        public int IdObjectType { get; set; }

        public User EditedBy { get; set; }

        public ICollection<TOptionValue> OptionValues { get; set; }

        public ICollection<TOptionType> OptionTypes { get; set; }
    }
}