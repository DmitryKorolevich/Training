using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerMapper: DynamicMapper<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>
    {
        private readonly CustomerAddressMapper _customerAddressMapper;
	    private readonly CustomerNoteMapper _customerNoteMapper;
        private readonly CustomerPaymentMethodMapper _paymentMethodMapper;

        public CustomerMapper(ITypeConverter converter,
            IModelConverterService converterService, CustomerAddressMapper customerAddressMapper,
            CustomerNoteMapper customerNoteMapper, IEcommerceRepositoryAsync<CustomerOptionType> customerRepositoryAsync, CustomerPaymentMethodMapper paymentMethodMapper)
            : base(converter, converterService, customerRepositoryAsync)
        {
            _customerAddressMapper = customerAddressMapper;
            _customerNoteMapper = customerNoteMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        protected override Expression<Func<CustomerOptionValue, int>> ObjectIdReferenceSelector
        {
            get { return c => c.IdCustomer; }
        }

        protected override async Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items,
            bool withDefaults = false)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Email = entity.Email;
                dynamic.PublicId = entity.PublicId;
                dynamic.IdDefaultPaymentMethod = entity.IdDefaultPaymentMethod;
                dynamic.IdAffiliate = entity.IdAffiliate;
                dynamic.ProfileAddress = await _customerAddressMapper.FromEntityAsync(entity.ProfileAddress, withDefaults);
                dynamic.ApprovedPaymentMethods = entity.PaymentMethods?.Select(p => p.IdPaymentMethod).ToList();
                dynamic.OrderNotes = entity.OrderNotes?.Select(p => p.IdOrderNote).ToList();

                dynamic.CustomerNotes.AddRange(await _customerNoteMapper.FromEntityRangeAsync(entity.CustomerNotes, withDefaults));
                dynamic.ShippingAddresses.AddRange(
                    await
                        _customerAddressMapper.FromEntityRangeAsync(entity.ShippingAddresses.Select(s => s.ShippingAddress).ToList(),
                            withDefaults));
                dynamic.CustomerPaymentMethods.AddRange(
                    await _paymentMethodMapper.FromEntityRangeAsync(entity.CustomerPaymentMethods, withDefaults));
                dynamic.Files = entity.Files;
            });
        }

        protected override async Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Email = dynamic.Email;
				entity.PublicId = dynamic.PublicId;
				entity.IdDefaultPaymentMethod = dynamic.IdDefaultPaymentMethod;
                entity.IdAffiliate = dynamic.IdAffiliate;

                entity.PaymentMethods.Merge(dynamic.ApprovedPaymentMethods, (method, i) => method.IdPaymentMethod != i,
                    c => new CustomerToPaymentMethod
                    {
                        IdCustomer = dynamic.Id,
                        IdPaymentMethod = c
                    });

                entity.OrderNotes.Merge(dynamic.OrderNotes, (note, i) => note.IdOrderNote != i,
                    c => new CustomerToOrderNote
                    {
                        IdCustomer = dynamic.Id,
                        IdOrderNote = c
                    });
                
                foreach (var note in dynamic.CustomerNotes)
                {
                    note.IdCustomer = dynamic.Id;
                }
                foreach (var paymentMethod in dynamic.CustomerPaymentMethods)
                {
                    paymentMethod.IdCustomer = dynamic.Id;
                }
                var addresses = entity.ShippingAddresses.Select(s => s.ShippingAddress).ToList();
                await _customerAddressMapper.SyncCollectionsAsync(dynamic.ShippingAddresses, addresses);
                await _customerNoteMapper.SyncCollectionsAsync(dynamic.CustomerNotes, entity.CustomerNotes);
                await
                    _paymentMethodMapper.SyncCollectionsAsync(dynamic.CustomerPaymentMethods,
                        entity.CustomerPaymentMethods);
                await _customerAddressMapper.UpdateEntityAsync(dynamic.ProfileAddress, entity.ProfileAddress);
            });
        }

        protected override async Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

	            if (entity.User == null)
	            {
		            entity.User = new User();
	            }
                entity.User.Id = dynamic.Id;
                entity.Email = dynamic.Email;
                entity.PublicId = dynamic.PublicId;
                entity.IdDefaultPaymentMethod = dynamic.IdDefaultPaymentMethod;
                entity.IdAffiliate = dynamic.IdAffiliate;

                entity.PaymentMethods = dynamic.ApprovedPaymentMethods?.Select(c => new CustomerToPaymentMethod()
                {
                    IdCustomer = dynamic.Id,
                    IdPaymentMethod = c
                }).ToList();

                entity.OrderNotes = dynamic.OrderNotes?.Select(c => new CustomerToOrderNote()
                {
                    IdCustomer = dynamic.Id,
                    IdOrderNote = c
                }).ToList();

                var addresses = await _customerAddressMapper.ToEntityRangeAsync(dynamic.ShippingAddresses);
                foreach (var address in addresses)
                {
                    entity.ShippingAddresses.Add(new CustomerToShippingAddress
                    {
                        IdCustomer = dynamic.Id,
                        ShippingAddress = address
                    });
                }
                entity.ProfileAddress = await _customerAddressMapper.ToEntityAsync(dynamic.ProfileAddress);
                entity.CustomerNotes = await _customerNoteMapper.ToEntityRangeAsync(dynamic.CustomerNotes);
				entity.Files = dynamic.Files;
			});
        }
    }
}
