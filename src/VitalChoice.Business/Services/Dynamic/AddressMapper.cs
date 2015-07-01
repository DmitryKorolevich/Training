using System;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces.Services;
using VitalChoice.DynamicData.Services;

namespace VitalChoice.Business.Services.Dynamic
{
    public class AddressMapper : DynamicObjectMapper<AddressDynamic, Address, AddressOptionType, AddressOptionValue>
    {
        public AddressMapper(IIndex<Type, IDynamicToModelMapper> mappers, IModelToDynamicContainer container) : base(mappers, container)
        {
        }

        protected override void FromEntity(AddressDynamic dynamic, Address entity, bool withDefaults = false)
        {
            dynamic.IdCustomer = entity.IdCustomer;
            dynamic.IdCountry = entity.IdCountry;
            dynamic.County = entity.County;
            dynamic.IdState = entity.IdState;
            dynamic.AddressType = entity.IdAddressType;
        }

        protected override void UpdateEntityInternal(AddressDynamic dynamic, Address entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.IdCountry = dynamic.IdCountry;
            entity.County = dynamic.County;
            entity.IdState = dynamic.IdState;
            entity.IdAddressType = dynamic.AddressType;
            foreach (var value in entity.OptionValues)
            {
                value.IdAddress = dynamic.Id;
            }
        }

        protected override void FillNewEntity(AddressDynamic dynamic, Address entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.IdCountry = dynamic.IdCountry;
            entity.County = dynamic.County;
            entity.IdState = dynamic.IdState;
            entity.IdAddressType = dynamic.AddressType;
        }
    }
}