using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VitalChoice.Business.Queries.Order
{
    public class OrderNoteQuery:QueryObject<OrderNote>
    {
		public OrderNoteQuery NotDeleted()
		{
			Add(x => x.StatusCode != RecordStatusCode.Deleted);

			return this;
		}

		public OrderNoteQuery MatchByid(int id)
		{
			Add(x => x.Id == id);

			return this;
		}

		public OrderNoteQuery MatchBySearchText(string keyword)
		{
			if (!string.IsNullOrWhiteSpace(keyword))
			{
				Add(x => x.Title.ToLower().Contains(keyword.ToLower()) || x.Description.ToLower().Contains(keyword.ToLower()));
			}

			return this;
		}

		public OrderNoteQuery MatchByName(string name, int? id)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				Add(x => x.Title.ToLower().Equals(name.ToLower()));
			}
			if (id.HasValue)
			{
				Add(x => x.Id != id.Value);
			}

			return this;
		}

	    public OrderNoteQuery MatchByCustomerType(CustomerType customerType)
	    {
			Add(x => x.CustomerTypes.Select(y=>y.IdCustomerType).Contains((int)customerType));

			return this;
		}

		public OrderNoteQuery HasCustomerAssignments()
		{
			Add(x => x.Customers.Any());

			return this;
		}
	}
}
