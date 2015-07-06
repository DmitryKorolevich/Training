﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Order;

namespace VitalChoice.Interfaces.Services.Order
{
	public interface IOrderNoteService
	{
		Task<PagedList<ExtendedOrderNote>> GetOrderNotesAsync(FilterBase filter);

		Task<OrderNote> GetAsync(int id);

		Task<ExtendedOrderNote> GetExtendedAsync(int id);

		Task<OrderNote> AddOrderNoteAsync(OrderNote orderNote);

		Task<OrderNote> UpdateOrderNoteAsync(OrderNote orderNote);

		Task<bool> DeleteOrderNoteAsync(int id);
	}
}