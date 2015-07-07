using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class CustomerMapper: DynamicObjectMapper<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>
    {
        private readonly AddressMapper _addressMapper;
	    private readonly CustomerNoteMapper _customerNoteMapper;

	    public CustomerMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamic> container, AddressMapper addressMapper, CustomerNoteMapper customerNoteMapper) : base(mappers, container)
	    {
		    _addressMapper = addressMapper;
		    _customerNoteMapper = customerNoteMapper;
	    }

	    protected override void FromEntity(CustomerDynamic dynamic, Customer entity, bool withDefaults = false)
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
                    (addressDynamic, address) => new { addressDynamic, address });
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
                //entity.Addresses.AddRange(Addresses.Where(s => s.Id == 0).Select(s =>
                //{
                //	var sku = s.ToEntity();
                //	sku.IdCustomer = Id;
                //	return sku;
                //}));
            }
            else
            {
                foreach (var sku in entity.Addresses)
                {
                    sku.StatusCode = RecordStatusCode.Deleted;
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
				//entity.Addresses.AddRange(Addresses.Where(s => s.Id == 0).Select(s =>
				//{
				//	var sku = s.ToEntity();
				//	sku.IdCustomer = Id;
				//	return sku;
				//}));
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

        protected override void FillNewEntity(CustomerDynamic dynamic, Customer entity)
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

            //if (dynamic.CustomerNotes != null)
            //{
            //    foreach (var item in entity.CustomerNotes)
            //    {
            //        item.Id = 0;
            //        item.IdCustomer = dynamic.Id;
            //    }
            //    entity.CustomerNotes = dynamic.CustomerNotes.ToList();
            //}

            //entity.Addresses = Addresses?.Select(s => s.ToEntity(s.OptionTypes)).ToList() ?? new List<Address>();
        }
    }
}
