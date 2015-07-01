using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;

namespace VitalChoice.Business.Queries.Payment
{
    public class PaymentMethodQuery: QueryObject<PaymentMethod>
    {
		public PaymentMethodQuery NotDeleted()
		{
			Add(x => x.StatusCode != RecordStatusCode.Deleted);

			return this;
		}

		public PaymentMethodQuery MatchByCustomerType(CustomerType customerType)
		{
			Add(x => x.CustomerTypes.Select(y => y.IdCustomerType).Contains((int)customerType));

			return this;
		}

		public PaymentMethodQuery HasCustomerAssignments()
		{
			Add(x => x.Customers.Any());

			return this;
		}
	}
}
