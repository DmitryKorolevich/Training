using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.Queries.Orders
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
			Add(x => x.CustomerTypes.Any(c => c.IdCustomerType == (int)customerType));

			return this;
		}

		public OrderNoteQuery HasCustomerAssignments()
		{
			Add(x => x.Customers.Any());

			return this;
		}
	}
}
