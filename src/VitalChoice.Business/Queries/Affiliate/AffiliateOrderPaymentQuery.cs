using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Affiliate
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