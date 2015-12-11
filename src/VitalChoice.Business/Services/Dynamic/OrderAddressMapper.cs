using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderAddressMapper : DynamicMapper<AddressDynamic, OrderAddress, AddressOptionType, OrderAddressOptionValue>
    {
        public OrderAddressMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<AddressOptionType> optionTypeRepositoryAsync)
            : base(converter, converterService, optionTypeRepositoryAsync)
        {
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<AddressDynamic, OrderAddress>> items, bool withDefaults = false)
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

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<AddressDynamic, OrderAddress>> items)
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

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<AddressDynamic, OrderAddress>> items)
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

        protected override Expression<Func<OrderAddressOptionValue, int>> ObjectIdReferenceSelector
        {
            get { return a => a.IdOrderAddress; }
        }
    }
}
