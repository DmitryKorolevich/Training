using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
{
    public class VOrderQuery : QueryObject<VOrder>
    {
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
                Add(x => x.IdOrderSource == idOrderSource.Value);
            }
            return this;
        }

        public VOrderQuery WithPOrderType(int? pOrderType)
        {
            if (pOrderType.HasValue)
            {
                Add(x => x.POrderType == pOrderType.Value);
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