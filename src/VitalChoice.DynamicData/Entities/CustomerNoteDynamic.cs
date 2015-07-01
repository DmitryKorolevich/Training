﻿using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerNoteDynamic : MappedObject<CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
    {
		public int IdCustomer { get; set; }

		public string Note { get; set; }
    }
}
