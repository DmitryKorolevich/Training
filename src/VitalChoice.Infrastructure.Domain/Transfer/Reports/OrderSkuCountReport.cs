using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class OrderSkuCountReport
    {
        public OrderSkuCountReport()
        {
            Skus=new List<OrderSkuCountReportSkuItem>();
        }

        public int PerishableTotalOrders { get; set; }

        public int PerishableSku1Orders { get; set; }

        public decimal PerishableSku1OrderPercent { get; set; }

        public int PerishableSku2Orders { get; set; }

        public decimal PerishableSku2OrderPercent { get; set; }

        public int PerishableSku3Orders { get; set; }

        public decimal PerishableSku3OrderPercent { get; set; }

        public int PerishableSku4Orders { get; set; }

        public decimal PerishableSku4OrderPercent { get; set; }

        public int PerishableSku5Orders { get; set; }

        public decimal PerishableSku5OrderPercent { get; set; }

        public int NonPerishableTotalOrders { get; set; }

        public int NonPerishableSku1Orders { get; set; }

        public decimal NonPerishableSku1OrderPercent { get; set; }

        public int NonPerishableSku2Orders { get; set; }

        public decimal NonPerishableSku2OrderPercent { get; set; }

        public int NonPerishableSku3Orders { get; set; }

        public decimal NonPerishableSku3OrderPercent { get; set; }

        public int NonPerishableSku4Orders { get; set; }

        public decimal NonPerishableSku4OrderPercent { get; set; }

        public int NonPerishableSku5Orders { get; set; }

        public decimal NonPerishableSku5OrderPercent { get; set; }

        public ICollection<OrderSkuCountReportSkuItem> Skus { get; set; }
    }
}