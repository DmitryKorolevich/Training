﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerPaymentMethodMapper :
        DynamicMapper
            <CustomerPaymentMethodDynamic, CustomerPaymentMethod, CustomerPaymentMethodOptionType,
                CustomerPaymentMethodOptionValue>
    {
        private readonly CustomerAddressMapper _customerAddressMapper;

        public CustomerPaymentMethodMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<CustomerPaymentMethodOptionType> repo, CustomerAddressMapper customerAddressMapper)
            : base(converter, converterService, repo)
        {
            _customerAddressMapper = customerAddressMapper;
        }

        public override Expression<Func<CustomerPaymentMethodOptionValue, int>> ObjectIdSelector => p => p.IdCustomerPaymentMethod;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerPaymentMethodDynamic, CustomerPaymentMethod>> items,
            bool withDefaults = false)
        {
            return items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Address = await _customerAddressMapper.FromEntityAsync(entity.BillingAddress);
                dynamic.IdCustomer = entity.IdCustomer;
            });
        }

        protected override Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerPaymentMethodDynamic, CustomerPaymentMethod>> items)
        {
            return items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.BillingAddress = await _customerAddressMapper.ToEntityAsync(dynamic.Address);
                entity.IdCustomer = dynamic.IdCustomer;
            });
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerPaymentMethodDynamic, CustomerPaymentMethod>> items)
        {
            return
                _customerAddressMapper.UpdateEntityRangeAsync(
                    items.Select(
                        i => new DynamicEntityPair<AddressDynamic, Address>(i.Dynamic.Address, i.Entity.BillingAddress))
                        .ToList());
        }
    }
}