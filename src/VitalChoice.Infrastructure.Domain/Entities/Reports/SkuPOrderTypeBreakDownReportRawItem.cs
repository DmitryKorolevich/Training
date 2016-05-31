using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class SkuPOrderTypeBreakDownReportRawItem : Entity
    { 
        public long RowNumber { get; set; }

        public int IdOrder { get; set; }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; }

        public int? POrderType { get; set; }
    }
}
