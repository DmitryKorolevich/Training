using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.Queries.Affiliates
{
    public class AffiliateOrderPaymentQuery : QueryObject<AffiliateOrderPayment>
    {
        public AffiliateOrderPaymentQuery WithIdAffiliate(int IdAffiliate)
        {
            Add(x => x.IdAffiliate == IdAffiliate);
            return this;
        }

        public AffiliateOrderPaymentQuery WithFromDate(DateTime? from)
        {
            if (from.HasValue)
            {
                Add(x => x.Order.DateCreated >= from.Value);
            }
            return this;
        }

        public AffiliateOrderPaymentQuery WithToDate(DateTime? to)
        {
            if (to.HasValue)
            {
                Add(x => x.Order.DateCreated <= to.Value);
            }
            return this;
        }

        public AffiliateOrderPaymentQuery WithOrderStatus()
        {
            Add(x => x.Order.StatusCode != (int)RecordStatusCode.Deleted && (x.Order.OrderStatus== OrderStatus.Processed ||
            x.Order.OrderStatus == OrderStatus.Shipped || x.Order.OrderStatus == OrderStatus.Exported));
            return this;
        }

        public AffiliateOrderPaymentQuery WithPaymentStatus(AffiliateOrderPaymentStatus? status)
        {
            if (status.HasValue)
            {
                Add(x => x.Status == status.Value);
            }
            return this;
        }
    }
}