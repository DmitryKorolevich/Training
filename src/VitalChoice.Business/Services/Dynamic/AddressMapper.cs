using System;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class AddressMapper : DynamicObjectMapper<AddressDynamic, Address, AddressOptionType, AddressOptionValue>
    {
        public AddressMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<AddressOptionType> optionTypesRepositoryAsync)
            : base(mappers, container, optionTypesRepositoryAsync)
        {
        }

        public override IQueryObject<AddressOptionType> GetOptionTypeQuery(int? idType)
        {
            return new AddressOptionTypeQuery().WithType((AddressType?) idType);
        }

        protected override void FromEntityInternal(AddressDynamic dynamic, Address entity, bool withDefaults = false)
        {
            dynamic.IdCustomer = entity.IdCustomer;
            dynamic.IdCountry = entity.IdCountry;
            dynamic.County = entity.County;
            dynamic.IdState = entity.IdState;
        }

        protected override void UpdateEntityInternal(AddressDynamic dynamic, Address entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.IdCountry = dynamic.IdCountry;
            entity.County = dynamic.County;
            entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
            foreach (var value in entity.OptionValues)
            {
                value.IdAddress = dynamic.Id;
            }
        }

        protected override void ToEntityInternal(AddressDynamic dynamic, Address entity)
        {
            entity.IdCustomer = dynamic.IdCustomer;
            entity.IdCountry = dynamic.IdCountry;
            entity.County = dynamic.County;
            entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
			entity.StatusCode = RecordStatusCode.Active;
        }
    }
}