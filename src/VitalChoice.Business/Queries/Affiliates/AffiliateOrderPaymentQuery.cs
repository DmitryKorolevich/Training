using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Business.Queries.Affiliates
{
    public class AffiliateOrderPaymentQuery : QueryObject<AffiliateOrderPayment>
    {
        public AffiliateOrderPaymentQuery WithIdAffiliate(int idAffiliate)
        {
            Add(x => x.IdAffiliate == idAffiliate);
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

        public AffiliateOrderPaymentQuery WithPaymentStatus(AffiliateOrderPaymentStatus? status)
        {
            if (status.HasValue)
            {
                Add(x => x.Status == status.Value);
            }
            return this;
        }

        //public AffiliateOrderPaymentQuery WithIdCustomer(int idCustomer)
        //{
        //    Add(a => a.Order.IdCustomer == idCustomer);
        //    return this;
        //}

        //public AffiliateOrderPaymentQuery WithIdCustomers(ICollection<int> idCustomers)
        //{
        //    foreach (var id in idCustomers)
        //    {
        //        Add(a => a.Order.IdCustomer == id);
        //    }
        //    return this;
        //}

        public AffiliateOrderPaymentQuery WithActiveOrder()
        {
            Add(
                a =>
                    a.Order.StatusCode != (int) RecordStatusCode.Deleted &&
                    (a.Order.OrderStatus == OrderStatus.Processed || a.Order.OrderStatus == OrderStatus.Shipped ||
                     a.Order.OrderStatus == OrderStatus.Exported || a.Order.POrderStatus == OrderStatus.Processed ||
                     a.Order.POrderStatus == OrderStatus.Shipped || a.Order.POrderStatus == OrderStatus.Exported ||
                     a.Order.NPOrderStatus == OrderStatus.Processed || a.Order.NPOrderStatus == OrderStatus.Shipped ||
                     a.Order.NPOrderStatus == OrderStatus.Exported));
            return this;
        }
    }
}