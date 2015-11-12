using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities;
using VitalChoice.Data.Repositories.Specifics;

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
                dynamic.State = entity.State;
                dynamic.Country = entity.Сountry;
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
