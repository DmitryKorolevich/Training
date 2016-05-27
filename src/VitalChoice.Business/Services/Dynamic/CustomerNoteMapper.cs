﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Customers;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerNoteMapper : DynamicMapper<CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
    {
        public CustomerNoteMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<CustomerNoteOptionType> customerNoteRepositoryAsync)
            : base(converter, converterService, customerNoteRepositoryAsync)
        {
        }

        public override Func<CustomerNoteOptionType, int?, bool> FilterFunc => null;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerNoteDynamic, CustomerNote>> items, bool withDefaults = false)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.IdCustomer = entity.IdCustomer;
                dynamic.Note = entity.Note;
                dynamic.IdAddedBy = entity.IdAddedBy;
            });
            return TaskCache.CompletedTask;
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerNoteDynamic, CustomerNote>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.IdCustomer = dynamic.IdCustomer;
                entity.Note = dynamic.Note;
                entity.DateEdited = dynamic.DateEdited;
                entity.IdEditedBy = dynamic.IdEditedBy;
            });
            return TaskCache.CompletedTask;
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerNoteDynamic, CustomerNote>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.IdCustomer = dynamic.IdCustomer;
                entity.Note = dynamic.Note;
                entity.StatusCode = (int)RecordStatusCode.Active;
                entity.IdAddedBy = entity.IdEditedBy;
            });
            return TaskCache.CompletedTask;
        }
    }
}