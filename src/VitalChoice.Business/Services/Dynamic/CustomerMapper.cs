using System;
using System.Collections.Generic;
using System.Linq;
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

        public override IQueryObject<CustomerOptionType> GetOptionTypeQuery(int? idType)
        {
            return new CustomerOptionTypeQuery().WithType((CustomerType?)idType);
        }

        protected override void FromEntityInternal(CustomerDynamic dynamic, Customer entity, bool withDefaults = false)
        {
            dynamic.User = entity.User;
            dynamic.Email = entity.Email;
            dynamic.IdDefaultPaymentMethod = entity.IdDefaultPaymentMethod;

            dynamic.ApprovedPaymentMethods = entity.PaymentMethods?.Select(p => p.IdPaymentMethod).ToList();
            dynamic.OrderNotes = entity.OrderNotes?.Select(p => p.IdOrderNote).ToList();

			foreach (var customerNoteDynamic in entity.CustomerNotes.Select(x => _customerNoteMapper.FromEntity(x)))
			{
				dynamic.CustomerNotes.Add(customerNoteDynamic);
			}
			dynamic.Addresses = new List<AddressDynamic>();
            foreach (var addressDynamic in entity.Addresses.Select(address => _addressMapper.FromEntity(address)))
            {
                dynamic.Addresses.Add(addressDynamic);
            }

	        dynamic.SuspendUserAccount = entity.StatusCode == RecordStatusCode.NotActive;
		}

        protected override void UpdateEntityInternal(CustomerDynamic dynamic, Customer entity)
        {
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

            if (dynamic.Addresses != null && dynamic.Addresses.Any())
            {
                //Update existing
                var itemsToUpdate = dynamic.Addresses.Join(entity.Addresses, sd => sd.Id, s => s.Id,
                    (addressDynamic, address) => new {addressDynamic, address});
                foreach (var item in itemsToUpdate)
                {
                    _addressMapper.UpdateEntity(item.addressDynamic, item.address);
                }

                //Delete
                var toDelete = entity.Addresses.Where(e => dynamic.Addresses.All(s => s.Id != e.Id));
                foreach (var sku in toDelete)
                {
                    sku.StatusCode = RecordStatusCode.Deleted;
                }

                //Insert
                entity.Addresses.AddRange(dynamic.Addresses.Where(s => s.Id == 0).Select(a =>
                {
                    var address = _addressMapper.ToEntity(a);
                    address.IdCustomer = entity.Id;
                    return address;
                }));
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
					(customerNoteDynamic, customerNote) => new { customerNoteDynamic, customerNote });
				foreach (var item in itemsToUpdate)
				{
					_customerNoteMapper.UpdateEntity(item.customerNoteDynamic, item.customerNote);
				}

				//Delete
				var toDelete = entity.CustomerNotes.Where(e => dynamic.CustomerNotes.All(s => s.Id != e.Id));
				foreach (var sku in toDelete)
				{
					sku.StatusCode = RecordStatusCode.Deleted;
				}

				//Insert
                entity.CustomerNotes.AddRange(dynamic.CustomerNotes.Where(s => s.Id == 0).Select(s =>
				{
					var customerNote = _customerNoteMapper.ToEntity(s);
					customerNote.IdCustomer = entity.Id;
					return customerNote;
				}));
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
        }

        protected override void ToEntityInternal(CustomerDynamic dynamic, Customer entity)
        {
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

            entity.Addresses = dynamic.Addresses?.Select(x => _addressMapper.ToEntity(x)).ToList() ?? new List<Address>();
            entity.CustomerNotes = dynamic.CustomerNotes?.Select(x => _customerNoteMapper.ToEntity(x)).ToList() ?? new List<CustomerNote>();

	        entity.StatusCode = dynamic.SuspendUserAccount ? RecordStatusCode.NotActive : RecordStatusCode.Active;
        }
    }
}
