using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;

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

        protected override Expression<Func<CustomerNoteOptionValue, int?>> ObjectIdReferenceSelector
        {
            get { return c => c.IdCustomerNote; }
        }

        public override IQueryOptionType<CustomerNoteOptionType> GetOptionTypeQuery()
        {
            return new CustomerNoteOptionTypeQuery();
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerNoteDynamic, CustomerNote>> items, bool withDefaults = false)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.IdCustomer = entity.IdCustomer;
                dynamic.Note = entity.Note;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerNoteDynamic, CustomerNote>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.IdCustomer = dynamic.IdCustomer;
                entity.Note = dynamic.Note;
            });
            return Task.Delay(0);
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
            });
            return Task.Delay(0);
        }
    }
}