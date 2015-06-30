using System.Collections.Generic;
using System.Linq;
using NuGet;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Users;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerDynamic : DynamicObject<Customer, CustomerOptionValue, CustomerOptionType>
    {
        public CustomerDynamic()
        {

        }

        public CustomerDynamic(Customer entity, bool withDefaults = false) : base(entity, withDefaults)
        {
        }

		public User User { get; set; }

		public string Email { get; set; }

		public CustomerType CustomerType { get; set; }

		public int IdDefaultPaymentMethod { get; set; }

		public ICollection<int> ApprovedPaymentMethods { get; set; }

		public ICollection<int> OrderNotes { get; set; }

		public ICollection<AddressDynamic> Addresses { get; set; }

		public ICollection<CustomerNote> CustomerNotes { get; set; }

		protected override void FillNewEntity(Customer entity)
        {
            entity.User = User;
            entity.Email = Email;
            entity.IdCustomerType = (int)CustomerType;
            entity.IdDefaultPaymentMethod = IdDefaultPaymentMethod;

			entity.PaymentMethods = ApprovedPaymentMethods?.Select(c => new CustomerToPaymentMethod()
			{
				IdCustomer = Id,
				IdPaymentMethod = c
			}).ToList();

			entity.OrderNotes = OrderNotes?.Select(c => new CustomerToOrderNote()
			{
				IdCustomer = Id,
				IdOrderNote = c
			}).ToList();

			if (CustomerNotes != null)
			{
				foreach (var item in CustomerNotes)
				{
					item.Id = 0;
					item.IdCustomer = Id;
				}
				entity.CustomerNotes = CustomerNotes.ToList();
			}

			//entity.Addresses = Addresses?.Select(s => s.ToEntity(s.OptionTypes)).ToList() ?? new List<Address>();
		}

        protected override void UpdateEntityInternal(Customer entity)
        {
			entity.User = User;
			entity.Email = Email;
			entity.IdCustomerType = (int)CustomerType;
			entity.IdDefaultPaymentMethod = IdDefaultPaymentMethod;

			entity.PaymentMethods = ApprovedPaymentMethods?.Select(c => new CustomerToPaymentMethod()
			{
				IdCustomer = Id,
				IdPaymentMethod = c
			}).ToList();

			entity.OrderNotes = OrderNotes?.Select(c => new CustomerToOrderNote()
			{
				IdCustomer = Id,
				IdOrderNote = c
			}).ToList();

			if (Addresses != null && Addresses.Any())
			{
				//Update existing
				var itemsToUpdate = Addresses.Join(entity.Addresses, sd => sd.Id, s => s.Id,
					(addressDynamic, address) => new { addressDynamic, address });
				foreach (var item in itemsToUpdate)
				{
					item.addressDynamic.UpdateEntity(item.address);
				}

				//Delete
				var toDelete = entity.Addresses.Where(e => Addresses.All(s => s.Id != e.Id));
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

	        if (CustomerNotes != null)
	        {
		        foreach (var item in CustomerNotes)
		        {
			        item.Id = 0;
			        item.IdCustomer = Id;
		        }
		        entity.CustomerNotes = CustomerNotes.ToList();
	        }

            //Set key on options
            foreach (var value in entity.OptionValues)
            {
                value.Id = Id;
            }

        }

        protected override void FromEntity(Customer entity, bool withDefaults = false)
        {
			User = entity.User;
			Email = entity.Email;
			CustomerType = (CustomerType)entity.IdCustomerType;
			IdDefaultPaymentMethod = entity.IdDefaultPaymentMethod;

			ApprovedPaymentMethods = entity.PaymentMethods?.Select(p => p.IdPaymentMethod).ToList();
            OrderNotes = entity.OrderNotes?.Select(p => p.IdOrderNote).ToList();
			CustomerNotes = entity.CustomerNotes?.ToList();
			Addresses = new List<AddressDynamic>();
			foreach (var addressDynamic in entity.Addresses.Select(address => new AddressDynamic(address)))
			{
				Addresses.Add(addressDynamic);
			}
        }
    }
}
