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

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerMapper: DynamicObjectMapper<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>
    {
        private readonly AddressMapper _addressMapper;
	    private readonly CustomerNoteMapper _customerNoteMapper;

        public CustomerMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> container, AddressMapper addressMapper,
            CustomerNoteMapper customerNoteMapper, IEcommerceRepositoryAsync<CustomerOptionType> customerRepositoryAsync)
            : base(mappers, container, customerRepositoryAsync)
        {
            _addressMapper = addressMapper;
            _customerNoteMapper = customerNoteMapper;
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
            
                dynamic.CustomerNotes.AddRange(await _customerNoteMapper.FromEntityRangeAsync(entity.CustomerNotes));                
                dynamic.Addresses.AddRange(await _addressMapper.FromEntityRangeAsync(entity.Addresses));
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

                if (dynamic.Addresses != null && dynamic.Addresses.Any())
                {
                    //Update existing
                    var itemsToUpdate = dynamic.Addresses.Join(entity.Addresses, addressDynamic => addressDynamic.Id, address => address.Id,
                        (addressDynamic, address) => new DynamicEntityPair<AddressDynamic, Address>(addressDynamic, address)).ToList();
                    await _addressMapper.UpdateEntityRangeAsync(itemsToUpdate);
                    foreach (var item in itemsToUpdate)
                    {
                        item.Entity.IdCustomer = dynamic.Id;
                    }

                    //Delete
                    var toDelete = entity.Addresses.Where(e => dynamic.Addresses.All(s => s.Id != e.Id));
                    foreach (var sku in toDelete)
                    {
                        sku.StatusCode = RecordStatusCode.Deleted;
                    }

                    //Insert
                    var addresses = await _addressMapper.ToEntityRangeAsync(dynamic.Addresses.Where(s => s.Id == 0).ToList());
                    foreach (var address in addresses)
                    {
                        address.IdCustomer = entity.Id;
                    }
                    entity.Addresses.AddRange(addresses);
                }
                else
                {
                    foreach (var address in entity.Addresses)
                    {
                        address.StatusCode = RecordStatusCode.Deleted;
                    }
                }

                if (dynamic.CustomerNotes != null && dynamic.CustomerNotes.Any())
                {
                    //Update existing
                    var itemsToUpdate = dynamic.CustomerNotes.Join(entity.CustomerNotes, sd => sd.Id, s => s.Id,
                        (customerNoteDynamic, customerNote) => new DynamicEntityPair<CustomerNoteDynamic, CustomerNote>(customerNoteDynamic, customerNote)).ToList();
                    await _customerNoteMapper.UpdateEntityRangeAsync(itemsToUpdate);
                    foreach (var item in itemsToUpdate)
                    {
                        item.Entity.IdCustomer = dynamic.Id;
                        item.Entity.StatusCode = RecordStatusCode.Active;
                    }

                    //Delete
                    var toDelete = entity.CustomerNotes.Where(e => dynamic.CustomerNotes.All(s => s.Id != e.Id));
                    foreach (var note in toDelete)
                    {
                        note.StatusCode = RecordStatusCode.Deleted;
                    }

                    //Insert
                    var notes = await _customerNoteMapper.ToEntityRangeAsync(dynamic.CustomerNotes.Where(s => s.Id == 0).ToList());
                    foreach (var customerNote in notes)
                    {
                        customerNote.IdCustomer = entity.Id;
                    }
                    entity.CustomerNotes.AddRange(notes);
                }
                else
                {
                    foreach (var customerNote in entity.CustomerNotes)
                    {
                        customerNote.StatusCode = RecordStatusCode.Deleted;
                    }
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
