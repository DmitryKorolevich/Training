using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class OrderSkuCountReportSkuItem
    {
        public OrderSkuCountReportSkuItem()
        {
            Orders = new List<OrderSkuCountReportOrderItem>();
        }

        public int IdSku { get; set; }

        public string Code { get; set; }

        public int TotalOrders { get; set; }

        public int Sku1Orders { get; set; }

        public decimal Sku1OrderPercent { get; set; }

        public int Sku2Orders { get; set; }

        public decimal Sku2OrderPercent { get; set; }

        public int Sku3Orders { get; set; }

        public decimal Sku3OrderPercent { get; set; }

        public int Sku4Orders { get; set; }

        public decimal Sku4OrderPercent { get; set; }

        public int Sku5Orders { get; set; }

        public decimal Sku5OrderPercent { get; set; }

        public ICollection<OrderSkuCountReportOrderItem> Orders { get; set; }
    }
}