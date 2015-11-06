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

namespace VitalChoice.Business.Queries.Order
{
    public class OrderQuery : QueryObject<VitalChoice.Domain.Entities.eCommerce.Orders.Order>
    {
        public OrderQuery WithCustomerId(int? idCustomer)
		{
			if (idCustomer.HasValue)
			{
				Add(x => x.IdCustomer == idCustomer.Value);
			}
			return this;
		}

		public OrderQuery WithActualStatusOnly()
		{
			Add(x => x.OrderStatus == OrderStatus.Exported || x.OrderStatus == OrderStatus.Processed || x.OrderStatus == OrderStatus.Shipped || x.OrderStatus == OrderStatus.ShipDelayed);

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