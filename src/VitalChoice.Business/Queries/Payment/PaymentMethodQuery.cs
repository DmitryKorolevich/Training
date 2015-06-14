using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Payment;

namespace VitalChoice.Business.Queries.Payment
{
    public class PaymentMethodQuery: QueryObject<PaymentMethod>
    {
		public PaymentMethodQuery NotDeleted()
		{
			Add(x => x.RecordStatusCode != RecordStatusCode.Deleted);

			return this;
		}
	}
}
