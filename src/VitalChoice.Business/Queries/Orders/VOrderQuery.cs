using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Queries.Orders
{
    public class VOrderQuery : QueryObject<VOrder>
    {
        public VOrderQuery WithCustomerId(int? idCustomer)
        {
            if (idCustomer.HasValue)
            {
                Add(x => x.IdCustomer == idCustomer.Value);
            }
            return this;
        }

		public VOrderQuery NotAutoShip()
		{
			Add(x => x.IdObjectType != OrderType.AutoShip);
			return this;
		}

		public VOrderQuery WithCreatedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                Add(x => x.DateCreated>= from.Value && x.DateCreated<= to.Value);
            }
            return this;
        }

        public VOrderQuery WithShippedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                Add(x => (x.DateShipped >= from.Value && x.DateShipped <= to.Value) ||
                    (x.PDateShipped >= from.Value && x.PDateShipped <= to.Value) ||
                    (x.NPDateShipped >= from.Value && x.NPDateShipped <= to.Value));
            }
            return this;
        }

        public VOrderQuery WithOrderStatus(OrderStatus? orderStatus)
        {
            if (orderStatus.HasValue)
            {
                Add(x => x.OrderStatus== orderStatus.Value || x.POrderStatus == orderStatus.Value || x.NPOrderStatus == orderStatus.Value);
            }
            return this;
        }

        public VOrderQuery WithoutIncomplete(OrderStatus? orderStatus, bool ignoreNotShowingIncomplete=false)
        {
            if (!ignoreNotShowingIncomplete)
            {
                if (!orderStatus.HasValue || orderStatus != OrderStatus.Incomplete)
                {
                    Add(x => (x.OrderStatus != OrderStatus.Incomplete && !x.POrderStatus.HasValue && !x.NPOrderStatus.HasValue)
                            || (!x.OrderStatus.HasValue && (x.POrderStatus != OrderStatus.Incomplete || x.NPOrderStatus != OrderStatus.Incomplete)));
                }
            }
            return this;
        }

        public VOrderQuery WithOrderType(OrderType? idObjectType)
        {
            if (idObjectType.HasValue)
            {
                Add(x => x.IdObjectType == idObjectType.Value);
            }
            return this;
        }

        public VOrderQuery WithOrderSource(int? idOrderSource)
        {
            if (idOrderSource.HasValue)
            {
                Add(x => x.SIdOrderSource == idOrderSource.Value.ToString());
            }
            return this;
        }

        public VOrderQuery WithPOrderType(int? pOrderType)
        {
            if (pOrderType.HasValue)
            {
                Add(x => x.SPOrderType == pOrderType.Value.ToString());
            }
            return this;
        }

        public VOrderQuery WithCustomerType(int? idCustomerType)
        {
            if (idCustomerType.HasValue)
            {
                Add(x => x.IdCustomerType == idCustomerType.Value);
            }
            return this;
        }

        public VOrderQuery WithShippingMethod(int? idShippingMethod)
        {
            if (idShippingMethod.HasValue)
            {
                Add(x => x.IdShippingMethod == idShippingMethod.Value);
            }
            return this;
        }

        public VOrderQuery WithId(string idString)
        {
            if (!String.IsNullOrEmpty(idString))
            {
                Add(x => x.IdString.Contains(idString));
            }
            return this;
        }
    }
}