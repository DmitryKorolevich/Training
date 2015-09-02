using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Data.Repositories.Specifics;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderPaymentMethodMapper :
        DynamicObjectMapper
            <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType,
                OrderPaymentMethodOptionValue>
    {
        private readonly OrderAddressMapper _orderAddressMapper;

        public override Expression<Func<OrderPaymentMethodOptionValue, int?>> ObjectIdSelector
            => v => v.IdOrderPaymentMethod;

        public OrderPaymentMethodMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<TypePair, IModelToDynamicConverter> converters,
            IEcommerceRepositoryAsync<CustomerPaymentMethodOptionType> optionTypeRepositoryAsync,
            OrderAddressMapper orderAddressMapper) : base(mappers, converters, optionTypeRepositoryAsync)
        {
            _orderAddressMapper = orderAddressMapper;
        }

        protected override async Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items,
            bool withDefaults = false)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Address = await _orderAddressMapper.FromEntityAsync(entity.BillingAddress);
                dynamic.IdOrder = entity.IdOrder;
            });
        }

        protected override async Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                if (dynamic.Address != null)
                {
                    entity.BillingAddress = await _orderAddressMapper.ToEntityAsync(dynamic.Address);
                    entity.BillingAddress.IdOrder = dynamic.IdOrder;
                }
                entity.IdOrder = dynamic.IdOrder;
            });
        }

        protected override async Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items)
        {
            await
                _orderAddressMapper.UpdateEntityRangeAsync(
                    items.Select(
                        i =>
                            new DynamicEntityPair<OrderAddressDynamic, OrderAddress>(i.Dynamic.Address,
                                i.Entity.BillingAddress))
                        .ToList());
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;
                if (dynamic.Address != null)
                {
                    entity.BillingAddress = await _orderAddressMapper.ToEntityAsync(dynamic.Address);
                    entity.BillingAddress.IdOrder = dynamic.IdOrder;
                }
                entity.IdOrder = dynamic.IdOrder;
                foreach (var value in entity.OptionValues)
                {
                    value.IdOrderPaymentMethod = dynamic.Id;
                }
            });
        }
    }
}
