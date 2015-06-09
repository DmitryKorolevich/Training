﻿using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.eCommerce.Base
{
    public abstract class DynamicDataEntity<TOptionValue, TOptionType> : Entity
        where TOptionValue: OptionValue<TOptionType>
        where TOptionType : OptionType
    {
        public RecordStatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public ICollection<TOptionValue> OptionValues { get; set; }

        public ICollection<TOptionType> OptionTypes { get; set; }
    }
}