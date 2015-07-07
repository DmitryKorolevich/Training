using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerNoteMapper : DynamicObjectMapper<CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
    {
        public CustomerNoteMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamic> container) : base(mappers, container)
        {
        }

        protected override void FromEntity(CustomerNoteDynamic dynamic, CustomerNote entity, bool withDefaults = false)
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

        protected override void FillNewEntity(CustomerNoteDynamic dynamic, CustomerNote entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.Note = dynamic.Note;
        }
    }
}
