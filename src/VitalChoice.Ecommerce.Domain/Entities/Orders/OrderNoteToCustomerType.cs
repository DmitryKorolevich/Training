﻿using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderNoteToCustomerType: Entity
    {
	    public OrderNote OrderNote { get; set; }

	    public CustomerTypeEntity CustomerType { get; set; }

	    public int IdOrderNote { get; set; }

	    public int IdCustomerType { get; set; }
    }
}