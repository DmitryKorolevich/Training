using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Business.Queries.Orders
{
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
                Add(x => x.Id == id.Value);
            }
            return this;
        }

        public OrderQuery WithCreatedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                Add(x => x.DateCreated >= from.Value && x.DateCreated <= to.Value);
            }
            return this;
        }

        public OrderQuery WithShippedDate(DateTime? from, DateTime? to)
        {
            if (from.HasValue && to.HasValue)
            {
                Add(x => x.OrderShippingPackages.Any(p=>p.ShippedDate>=from.Value && p.ShippedDate <= to.Value));
            }
            return this;
        }

        public OrderQuery WithOrderType(OrderType? idObjectType)
        {
            if (idObjectType.HasValue)
            {
                Add(x => x.IdObjectType == (int)idObjectType.Value);
            }
            return this;
        }

        public OrderQuery WithOrderTypes(ICollection<OrderType> idObjectTypes)
        {
            if (idObjectTypes!=null && idObjectTypes.Count>0)
            {
                var ids = idObjectTypes.Select(p => (int) p).ToArray();
                Add(x => ids.Contains(x.IdObjectType));
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

        public OrderQuery WithoutIncomplete(OrderStatus? orderStatus = null, bool ignoreNotShowingIncomplete = false)
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

		public OrderQuery NotAutoShip()
		{
			Add(x => x.IdObjectType != (int)OrderType.AutoShip);
			return this;
		}

        public OrderQuery WithReshipServiceCode(int? serviceCode)
        {
            if (serviceCode.HasValue)
            {
                Add(c => c.WhenValues(new
                {
                    ServiceCode = serviceCode.Value
                }, (int)OrderType.Reship, ValuesFilterType.And));
            }
            return this;
        }

        public OrderQuery WithRefundServiceCode(int? serviceCode)
        {
            if (serviceCode.HasValue)
            {
                Add(c => c.WhenValues(new
                {
                    ServiceCode = serviceCode.Value
                }, (int)OrderType.Refund, ValuesFilterType.And));
            }
            return this;
        }

        public OrderQuery WithOrderDynamicValues(int? idOrderSource, int? pOrderType, int? idShippingMethod)
        {
            if (idOrderSource.HasValue)
            {
                Add(c => c.WhenValues(new
                {
                    OrderType= idOrderSource.Value
                }, ValuesFilterType.And));
            }
            if (pOrderType.HasValue)
            {
                Add(c => c.WhenValues(new
                {
                    POrderType = pOrderType.Value
                }, ValuesFilterType.And));
            }
            if (idShippingMethod.HasValue && idShippingMethod.Value==1)//upgraded
            {
                Add(c => c.WhenValues(new
                {
                    ShippingUpgradeP = 1
                }, ValuesFilterType.And) ||
                c.WhenValues(new
                {
                    ShippingUpgradeP = 2
                }, ValuesFilterType.And) ||
                c.WhenValues(new
                {
                    ShippingUpgradeNP = 1
                }, ValuesFilterType.And) ||
                c.WhenValues(new
                {
                    ShippingUpgradeNP = 2
                }, ValuesFilterType.And));
            }
            return this;
        }

        public OrderQuery WithCustomerDynamicValues(string firstName, string lastName, string company)
        {
            if (!string.IsNullOrEmpty(firstName))
            {
                Add(c => c.Customer.ProfileAddress.WhenValues(new
                {
                    FirstName = firstName
                }, (int)AddressType.Profile, ValuesFilterType.And));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                Add(c => c.Customer.ProfileAddress.WhenValues(new
                {
                    LastName = lastName
                },(int) AddressType.Profile, ValuesFilterType.And));
            }
            if (!string.IsNullOrEmpty(company))
            {
                Add(c => c.Customer.ProfileAddress.WhenValues(new
                {
                    Company = company
                }, (int)AddressType.Profile, ValuesFilterType.And));
            }

            return this;
        }

        public OrderQuery WithIdSku(int? idSku)
        {
            if (idSku.HasValue)
            {
                Add(c => c.Skus.Any(p => p.IdSku==idSku.Value) || c.PromoSkus.Any(p => p.IdSku == idSku.Value && !p.Disabled));
            }

            return this;
        }

        public OrderQuery WithShipState(int? idShipState)
        {
            if (idShipState.HasValue)
            {
                Add(x => x.ShippingAddress.IdState== idShipState.Value);
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

        public OrderQuery WithCreatedByAgentsOrWithout(ICollection<int> agentIds)
        {
            if (agentIds!=null)
            {
                var tAgentIds = agentIds.Select(p => (int?) p).ToList();
                Add(x => !x.IdAddedBy.HasValue || tAgentIds.Contains(x.IdAddedBy));
            }
            return this;
        }

        public OrderQuery Active()
        {
            Add(a => a.StatusCode != (int)RecordStatusCode.Deleted &&
                    (a.OrderStatus == OrderStatus.Processed || a.OrderStatus == OrderStatus.Shipped ||
                     a.OrderStatus == OrderStatus.Exported || a.POrderStatus == OrderStatus.Processed ||
                     a.POrderStatus == OrderStatus.Shipped || a.POrderStatus == OrderStatus.Exported ||
                     a.NPOrderStatus == OrderStatus.Processed || a.NPOrderStatus == OrderStatus.Shipped ||
                     a.NPOrderStatus == OrderStatus.Exported));
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