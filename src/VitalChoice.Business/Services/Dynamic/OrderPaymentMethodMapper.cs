﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderPaymentMethodMapper :
        DynamicMapper
            <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType,
                OrderPaymentMethodOptionValue>
    {
        private readonly OrderAddressMapper _orderAddressMapper;

        public OrderPaymentMethodMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<CustomerPaymentMethodOptionType> optionTypeRepositoryAsync,
            OrderAddressMapper orderAddressMapper) : base(converter, converterService, optionTypeRepositoryAsync)
        {
            _orderAddressMapper = orderAddressMapper;
        }

        public override Expression<Func<OrderPaymentMethodOptionValue, int>> ObjectIdSelector => p => p.IdOrderPaymentMethod;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items,
            bool withDefaults = false)
        {
            return items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Address = await _orderAddressMapper.FromEntityAsync(entity.BillingAddress);
            });
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items)
        {
            return
                _orderAddressMapper.UpdateEntityRangeAsync(
                    items.Select(
                        i =>
                            new DynamicEntityPair<AddressDynamic, OrderAddress>(i.Dynamic.Address,
                                i.Entity.BillingAddress))
                        .ToList());
        }

        protected override Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderPaymentMethodDynamic, OrderPaymentMethod>> items)
        {
            return items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;
                if (dynamic.Address != null)
                {
                    entity.BillingAddress = await _orderAddressMapper.ToEntityAsync(dynamic.Address);
                }
                foreach (var value in entity.OptionValues)
                {
                    value.IdOrderPaymentMethod = dynamic.Id;
                }
            });
        }
    }
}
