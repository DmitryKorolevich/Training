using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using Autofac.Features.Indexed;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

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

        public override Expression<Func<CustomerPaymentMethodOptionValue, int?>> ObjectIdSelector
        {
            get { return v => v.IdCustomerPaymentMethod; }
        }

        protected override async Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerPaymentMethodDynamic, CustomerPaymentMethod>> items,
            bool withDefaults = false)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Address = await _customerAddressMapper.FromEntityAsync(entity.BillingAddress);
                dynamic.IdCustomer = entity.IdCustomer;
            });
        }

        protected override async Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerPaymentMethodDynamic, CustomerPaymentMethod>> items)
        {
            await items.ForEachAsync(async pair =>
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