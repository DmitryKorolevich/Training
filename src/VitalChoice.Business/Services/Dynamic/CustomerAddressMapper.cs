using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerAddressMapper : DynamicObjectMapper<CustomerAddressDynamic, Address, AddressOptionType, AddressOptionValue>
    {
        public CustomerAddressMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<TypePair, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<AddressOptionType> optionTypesRepositoryAsync)
            : base(mappers, container, optionTypesRepositoryAsync)
        {
        }

        public override Expression<Func<AddressOptionValue, int?>> ObjectIdSelector
        {
            get { return a => a.IdAddress; }
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerAddressDynamic, Address>> items, bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.IdCustomer = entity.IdCustomer;
                dynamic.IdCountry = entity.IdCountry;
                dynamic.County = entity.County;
                dynamic.IdState = entity.IdState;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerAddressDynamic, Address>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdCustomer = dynamic.IdCustomer;
                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.StatusCode = RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerAddressDynamic, Address>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdCustomer = dynamic.IdCustomer;
                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.StatusCode = RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }
    }
}