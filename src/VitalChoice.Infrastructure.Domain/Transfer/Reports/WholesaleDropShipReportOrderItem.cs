using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class WholesaleDropShipReportOrderItem
    {
        public WholesaleDropShipReportOrderItem()
        {
            Skus = new List<WholesaleDropShipReportSkuItem>();
        }

        public int IdOrder { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public decimal Shipping { get; set; }

        public decimal Total { get; set; }

        public string OrderNotes { get; set; }

        public string PoNumber { get; set; }

        public string ShippingCompany { get; set; }

        public string ShippingFirstName { get; set; }

        public string ShippingLastName { get; set; }

        public string ShippingAddress1 { get; set; }

        public string ShippingAddress2 { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string StateCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public DateTime? ShipDate { get; set; }

        public string ShippingIdConfirmation { get; set; }

        public string ShippingCarrier { get; set; }

        public ICollection<WholesaleDropShipReportSkuItem> Skus { get; set; }
    }
}