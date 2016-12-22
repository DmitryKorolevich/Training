using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class SkuAverageDailySalesReportRawItem : Entity
    { 
        public decimal TotalAmount { get; set; }

        public int TotalQuantity { get; set; }

        public string ProductCategories { get; set; }

        public string Code { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public int IdProduct { get; set; }

        public RecordStatusCode ProductStatusCode { get; set; }

        public int? ProductIdVisibility { get; set; }

        public int ProductIdObjectType { get; set; }

        public string Name { get; set; }

        public string SubTitle { get; set; }

        public int? QTY { get; set; }

        public bool? DisregardStock { get; set; }

        public int? Stock { get; set; }
    }
}