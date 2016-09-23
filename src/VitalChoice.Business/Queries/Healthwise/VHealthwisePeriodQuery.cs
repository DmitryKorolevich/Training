using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;

namespace VitalChoice.Business.Queries.Healthwise
{
    public class VHealthwisePeriodQuery : QueryObject<VHealthwisePeriod>
    {
        public VHealthwisePeriodQuery WithAllowPaymentOnly(bool notBilled, int? ordersCount)
        {
            if (notBilled && ordersCount.HasValue)
            {
                Add(x => x.PaidDate == null && x.OrdersCount >= ordersCount.Value);
            }
            return this;
        }

        public VHealthwisePeriodQuery WithDateTo(DateTime? to)
        {
            if (to.HasValue)
            {
                Add(x => x.StartDate<=to.Value);
            }
            return this;
        }

        public VHealthwisePeriodQuery WithDateFrom(DateTime? from)
        {
            if (from.HasValue)
            {
                Add(x => x.EndDate >= from.Value);
            }
            return this;
        }

        public VHealthwisePeriodQuery WithCustomerId(int? idCustomer)
        {
            if (idCustomer.HasValue)
            {
                Add(x => x.IdCustomer == idCustomer.Value);
            }
            return this;
        }

        public VHealthwisePeriodQuery WithCustomerFirstName(string firstName)
        {
            if (!string.IsNullOrEmpty(firstName))
            {
                Add(x => x.CustomerFirstName.StartsWith(firstName));
            }
            return this;
        }

        public VHealthwisePeriodQuery WithCustomerLastName(string lastName)
        {
            if (!string.IsNullOrEmpty(lastName))
            {
                Add(x => x.CustomerLastName.StartsWith(lastName));
            }
            return this;
        }

        public VHealthwisePeriodQuery WithNotPaid(bool notPaid)
        {
            if (notPaid)
            {
                Add(x => x.PaidDate == null);
            }
            return this;
        }
    }
}
