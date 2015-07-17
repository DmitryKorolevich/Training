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
    public class CustomerNoteMapper : DynamicObjectMapper<CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
    {
        public CustomerNoteMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<CustomerNoteOptionType> customerNoteRepositoryAsync)
            : base(mappers, container, customerNoteRepositoryAsync)
        {
        }

        public override Expression<Func<CustomerNoteOptionValue, int?>> ObjectIdSelector
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
                foreach (var value in entity.OptionValues)
                {
                    value.IdCustomerNote = dynamic.Id;
                }
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
                entity.StatusCode = RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }
    }
}