using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.DynamicData.Extensions;

namespace VitalChoice.Business.Queries.Orders
{
    public class OrderDynamicFilter
    {
        public int? OrderType { get; set; }

        public int? POrderType { get; set; }

        public int? ShippingUpgradeP { get; set; }

        public int? ShippingUpgradeNP { get; set; }
    }

    public class OrderQuery : QueryObject<Order>
    {
        public OrderQuery WithCustomerId(int? idCustomer)
		{
			if (idCustomer.HasValue)
			{
				Add(x => x.IdCustomer == idCustomer.Value);
			}
			return this;
		}

        public OrderQuery WithCustomerIds(ICollection<int> ids)
        {
            Add(x => (ids ?? new List<int>()).Contains(x.IdCustomer));
            return this;
        }

        public OrderQuery WithActualStatusOnly()
		{
			Add(x => x.OrderStatus == OrderStatus.Exported || x.OrderStatus == OrderStatus.Processed || x.OrderStatus == OrderStatus.Shipped || x.OrderStatus == OrderStatus.ShipDelayed || x.OrderStatus == OrderStatus.OnHold ||
                x.POrderStatus == OrderStatus.Exported || x.POrderStatus == OrderStatus.Processed || x.POrderStatus == OrderStatus.Shipped || x.POrderStatus == OrderStatus.ShipDelayed || x.POrderStatus == OrderStatus.OnHold ||
                x.NPOrderStatus == OrderStatus.Exported || x.NPOrderStatus == OrderStatus.Processed || x.NPOrderStatus == OrderStatus.Shipped || x.NPOrderStatus == OrderStatus.ShipDelayed || x.NPOrderStatus == OrderStatus.OnHold);

			return this;
		}

		public OrderQuery FilterById(string id)
		{
			if (!string.IsNullOrWhiteSpace(id))
			{
				Add(p => p.Id.ToString().Contains(id));
			}
			return this;
		}

		public OrderQuery NotDeleted()
		{
			Add(x => x.StatusCode != (int)RecordStatusCode.Deleted);

			return this;
		}

        public OrderQuery WithId(int? id)
        {
            if (id.HasValue)
            {
                Add(x => x.Id >= id.Value);
            }
            return this;
        }

        public OrderQuery WithCreatedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue)
            {
                Add(x => x.DateCreated >= from.Value);
            }
            if (to.HasValue)
            {
                Add(x =>x.DateCreated <= to.Value);
            }
            return this;
        }

        public OrderQuery WithShippedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue)
            {
                //
            }
            if (to.HasValue)
            {
            }
            return this;
        }

        public OrderQuery WithOrderStatus(OrderStatus? orderStatus)
        {
            if (orderStatus.HasValue)
            {
                Add(x => x.OrderStatus == orderStatus.Value || x.POrderStatus == orderStatus.Value || x.NPOrderStatus == orderStatus.Value);
            }
            return this;
        }

        public OrderQuery WithoutIncomplete(OrderStatus? orderStatus, bool ignoreNotShowingIncomplete = false)
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

        public OrderQuery WithOrderdynamicValues(int? idOrderSource, int? pOrderType, int? idShippingMethod)
        {
            var filter = new OrderDynamicFilter();
            if (idOrderSource.HasValue)
            {
                filter.OrderType = idOrderSource.Value;
            }
            if (pOrderType.HasValue)
            {
                filter.POrderType = pOrderType.Value;
            }
            if (idShippingMethod.HasValue)
            {

            }
            if (idOrderSource.HasValue || pOrderType.HasValue || idShippingMethod.HasValue)
            {
                Add(c => c.WhenValues(filter));
            }
            return this;
        }

        public OrderQuery WithCustomerType(int? idCustomerType)
        {
            if (idCustomerType.HasValue)
            {
                Add(x => x.Customer.IdObjectType == idCustomerType.Value);
            }
            return this;
        }

        #region AffiliateOrders

        public OrderQuery WithIdAffiliate(int? IdAffiliate)
        {
            Add(x => x.AffiliateOrderPayment.IdAffiliate == IdAffiliate);
            return this;
        }

        public OrderQuery WithFromDate(DateTime? from)
        {
            if (from.HasValue)
            {
                Add(x => x.DateCreated >= from.Value);
            }
            return this;
        }

        public OrderQuery WithToDate(DateTime? to)
        {
            if (to.HasValue)
            {
                Add(x => x.DateCreated <= to.Value);
            }
            return this;
        }

        public OrderQuery WithAffiliateOrderStatus()
        {
            Add(x => x.StatusCode != (int)RecordStatusCode.Deleted && (x.OrderStatus == OrderStatus.Processed ||
            x.OrderStatus == OrderStatus.Shipped || x.OrderStatus == OrderStatus.Exported));
            return this;
        }

        public OrderQuery WithPaymentStatus(AffiliateOrderPaymentStatus? status)
        {
            if (status.HasValue)
            {
                Add(x => x.AffiliateOrderPayment.Status == status.Value);
            }
            return this;
        }

        #endregion
    }
}