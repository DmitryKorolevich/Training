using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VitalChoice.Business.Queries.Order
{
    public class OrderNoteQuery:QueryObject<OrderNote>
    {
		public OrderNoteQuery NotDeleted()
		{
			Add(x => x.RecordStatusCode != RecordStatusCode.Deleted);

			return this;
		}
	}
}
