using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerAddressMapper : DynamicMapper<AddressDynamic, Address, AddressOptionType, AddressOptionValue>
    {
        public CustomerAddressMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<AddressOptionType> optionTypesRepositoryAsync)
            : base(converter, converterService, optionTypesRepositoryAsync)
        {
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<AddressDynamic, Address>> items, bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.IdCountry = entity.IdCountry;
                dynamic.County = entity.County;
                dynamic.IdState = entity.IdState;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<AddressDynamic, Address>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.StatusCode = (int)RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<AddressDynamic, Address>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.StatusCode = (int)RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }
    }
}