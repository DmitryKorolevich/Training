﻿using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class OrderNoteToCustomerType
    {
	    public OrderNote OrderNote { get; set; }

	    public CustomerType CustomerType { get; set; }

	    public int IdOrderNote { get; set; }

	    public int IdCustomerType { get; set; }
    }
}