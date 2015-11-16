﻿using System;
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
                Add(x => x.DateShipped >= from.Value && x.DateShipped <= to.Value);
            }
            return this;
        }

        public VOrderQuery WithOrderStatus(OrderStatus? orderStatus)
        {
            if (orderStatus.HasValue)
            {
                Add(x => x.OrderStatus== orderStatus.Value);
            }
            return this;
        }

        public VOrderQuery WithoutIncomplete(OrderStatus? orderStatus)
        {
            if (!orderStatus.HasValue || orderStatus!= OrderStatus.Incomplete)
            {
                Add(x => x.OrderStatus != OrderStatus.Incomplete);
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
    }
}