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

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderAddressMapper : DynamicObjectMapper<OrderAddressDynamic, OrderAddress, AddressOptionType, OrderAddressOptionValue>
    {
        public OrderAddressMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> converters, IReadRepositoryAsync<AddressOptionType> optionTypeRepositoryAsync) : base(mappers, converters, optionTypeRepositoryAsync)
        {
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items, bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.IdOrder = entity.IdOrder;
                dynamic.IdCountry = entity.IdCountry;
                dynamic.County = entity.County;
                dynamic.IdState = entity.IdState;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdOrder = dynamic.IdOrder;
                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                foreach (var value in entity.OptionValues)
                {
                    value.IdOrderAddress = dynamic.Id;
                }
                entity.StatusCode = RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdOrder = dynamic.IdOrder;
                entity.IdCountry = dynamic.IdCountry;
                entity.County = dynamic.County;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.StatusCode = RecordStatusCode.Active;
            });
            return Task.Delay(0);
        }

        public override Expression<Func<OrderAddressOptionValue, int?>> ObjectIdSelector
        {
            get { return a => a.IdOrderAddress; }
        }
    }
}
