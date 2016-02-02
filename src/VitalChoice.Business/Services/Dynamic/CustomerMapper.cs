using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;

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

                entity.PaymentMethods.MergeKeyed(dynamic.ApprovedPaymentMethods, method => method.IdPaymentMethod, i => i,
                    c => new CustomerToPaymentMethod
                    {
                        IdCustomer = dynamic.Id,
                        IdPaymentMethod = c
                    });

                entity.OrderNotes.MergeKeyed(dynamic.OrderNotes, note => note.IdOrderNote, i => i,
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

                entity.ShippingAddresses.AddKeyed(addresses,
                    address => address.IdAddress, newAddress => newAddress.Id, dbAddress => new CustomerToShippingAddress
                    {
                        IdAddress = dbAddress.Id,
                        IdCustomer = dynamic.Id,
                        ShippingAddress = dbAddress
                    });

                await _customerNoteMapper.SyncCollectionsAsync(dynamic.CustomerNotes, entity.CustomerNotes);
                await
                    _paymentMethodMapper.SyncCollectionsAsync(dynamic.CustomerPaymentMethods,
                        entity.CustomerPaymentMethods);
                foreach (var paymentMethod in entity.CustomerPaymentMethods)
                {
                    if (paymentMethod.StatusCode == (int) RecordStatusCode.Deleted && paymentMethod.BillingAddress != null)
                    {
                        paymentMethod.BillingAddress.StatusCode = (int) RecordStatusCode.Deleted;
                    }
                }
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

                await _paymentMethodMapper.SyncCollectionsAsync(dynamic.CustomerPaymentMethods, entity.CustomerPaymentMethods);
                foreach (var paymentMethod in entity.CustomerPaymentMethods)
                {
                    if (paymentMethod.StatusCode == (int)RecordStatusCode.Deleted && paymentMethod.BillingAddress != null)
                    {
                        paymentMethod.BillingAddress.StatusCode = (int)RecordStatusCode.Deleted;
                    }
                }

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
