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

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerMapper: DynamicObjectMapper<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>
    {
        private readonly AddressMapper _addressMapper;
	    private readonly CustomerNoteMapper _customerNoteMapper;
        private readonly CustomerPaymentMethodMapper _paymentMethodMapper;

        public CustomerMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> container, AddressMapper addressMapper,
            CustomerNoteMapper customerNoteMapper, IEcommerceRepositoryAsync<CustomerOptionType> customerRepositoryAsync, CustomerPaymentMethodMapper paymentMethodMapper)
            : base(mappers, container, customerRepositoryAsync)
        {
            _addressMapper = addressMapper;
            _customerNoteMapper = customerNoteMapper;
            _paymentMethodMapper = paymentMethodMapper;
        }

        public override Expression<Func<CustomerOptionValue, int?>> ObjectIdSelector
        {
            get { return c => c.IdCustomer; }
        }

        protected async override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items, bool withDefaults = false)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.User = entity.User;
                dynamic.Email = entity.Email;
                dynamic.IdDefaultPaymentMethod = entity.IdDefaultPaymentMethod;

                dynamic.ApprovedPaymentMethods = entity.PaymentMethods?.Select(p => p.IdPaymentMethod).ToList();
                dynamic.OrderNotes = entity.OrderNotes?.Select(p => p.IdOrderNote).ToList();
            
                dynamic.CustomerNotes.AddRange(await _customerNoteMapper.FromEntityRangeAsync(entity.CustomerNotes, withDefaults));                
                dynamic.Addresses.AddRange(await _addressMapper.FromEntityRangeAsync(entity.Addresses, withDefaults));
                dynamic.CustomerPaymentMethods.AddRange(await _paymentMethodMapper.FromEntityRangeAsync(entity.CustomerPaymentMethods, withDefaults));
            });
        }

        protected async override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items)
        {
            await items.ForEachAsync(async pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Email = dynamic.Email;
                entity.IdDefaultPaymentMethod = dynamic.IdDefaultPaymentMethod;

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

                await _addressMapper.SyncCollectionsAsync(dynamic.Addresses, entity.Addresses);
                foreach (var address in entity.Addresses)
                {
                    address.IdCustomer = dynamic.Id;
                }
                await _customerNoteMapper.SyncCollectionsAsync(dynamic.CustomerNotes, entity.CustomerNotes);
                foreach (var note in entity.CustomerNotes)
                {
                    note.IdCustomer = dynamic.Id;
                }

                await
                    _paymentMethodMapper.SyncCollectionsAsync(dynamic.CustomerPaymentMethods,
                        entity.CustomerPaymentMethods);
                foreach (var paymentMethod in entity.CustomerPaymentMethods)
                {
                    paymentMethod.IdCustomer = dynamic.Id;
                }

                foreach (var value in entity.OptionValues)
                {
                    value.IdCustomer = dynamic.Id;
                }
            });
        }

        protected async override Task ToEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<CustomerDynamic, Customer>> items)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.User = dynamic.User;
                entity.Email = dynamic.Email;
                entity.IdDefaultPaymentMethod = dynamic.IdDefaultPaymentMethod;

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

                entity.Addresses = await _addressMapper.ToEntityRangeAsync(dynamic.Addresses);
                entity.CustomerNotes = await _customerNoteMapper.ToEntityRangeAsync(dynamic.CustomerNotes);
            });
        }
    }
}
