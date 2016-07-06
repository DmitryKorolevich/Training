using System;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Payment;

namespace VitalChoice.Business.Queries.Payment
{
    public class PaymentMethodQuery: QueryObject<PaymentMethod>
    {
		public PaymentMethodQuery NotDeleted()
		{
			Add(x => x.StatusCode != RecordStatusCode.Deleted);

			return this;
		}

		public PaymentMethodQuery MatchByCustomerType(CustomerType? customerType)
		{
            if (customerType.HasValue)
            {
                int customerTypeValue = (int) customerType.Value;
                Add(x => x.CustomerTypes.Any(c => c.IdCustomerType == customerTypeValue));
            }

			return this;
		}

		public PaymentMethodQuery HasCustomerAssignments()
		{
			Add(x => x.Customers.Any());

			return this;
		}

		public PaymentMethodQuery CreditCard()
		{
		    Add(x => x.Name == "Credit Card");

			return this;
		}
	}
}
