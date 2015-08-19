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

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderAddressMapper : DynamicObjectMapper<OrderAddressDynamic, OrderAddress, AddressOptionType, OrderAddressOptionValue>
    {
        public OrderAddressMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> converters, IReadRepositoryAsync<AddressOptionType> optionTypeRepositoryAsync) : base(mappers, converters, optionTypeRepositoryAsync)
        {
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items, bool withDefaults = false)
        {
            throw new NotImplementedException();
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items)
        {
            throw new NotImplementedException();
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderAddressDynamic, OrderAddress>> items)
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<OrderAddressOptionValue, int?>> ObjectIdSelector
        {
            get { return a => a.IdOrderAddress; }
        }
    }
}
