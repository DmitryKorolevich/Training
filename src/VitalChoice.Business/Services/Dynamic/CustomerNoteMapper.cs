using System;
using System.Collections.Generic;
using System.Linq;
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

        public override IQueryObject<CustomerNoteOptionType> GetOptionTypeQuery(int? idType)
        {
            return new CustomerNoteOptionTypeQuery();
        }

        protected override void FromEntityInternal(CustomerNoteDynamic dynamic, CustomerNote entity, bool withDefaults = false)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.Note = dynamic.Note;
        }

        protected override void UpdateEntityInternal(CustomerNoteDynamic dynamic, CustomerNote entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.Note = dynamic.Note;
            foreach (var value in entity.OptionValues)
            {
                value.IdCustomerNote = dynamic.Id;
            }
        }

        protected override void ToEntityInternal(CustomerNoteDynamic dynamic, CustomerNote entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.Note = dynamic.Note;
	        entity.StatusCode = RecordStatusCode.Active;
        }
    }
}
