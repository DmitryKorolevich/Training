﻿using System;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData
{
    public interface IDynamicEntity<TEntity, TOptionValue, TOptionType>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        void UpdateEntity(TEntity entity);
        TEntity ToEntity();
        int Id { get; set; }
        RecordStatusCode StatusCode { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateEdited { get; set; }
    }
}