using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.Queries.Orders
{
    public class OrderReviewRuleQuery : QueryObject<OrderReviewRule>
    {
		public OrderReviewRuleQuery NotDeleted()
		{
			Add(x => x.StatusCode != (int)RecordStatusCode.Deleted);
			return this;
		}

		public OrderReviewRuleQuery WithName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				Add(x => x.Name.Contains(name));
			}
			return this;
		}
	}
}
