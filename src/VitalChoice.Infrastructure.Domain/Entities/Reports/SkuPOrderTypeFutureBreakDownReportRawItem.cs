using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class SkuPOrderTypeFutureBreakDownReportRawItem : Entity
    { 
        public long RowNumber { get; set; }

        public int IdOrder { get; set; }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        public string Code { get; set; }

        public int Quantity { get; set; }

        public DateTime? ShipDelayDate { get; set; }

        public DateTime? ShipDelayDateP { get; set; }

        public DateTime? ShipDelayDateNP { get; set; }

        public int? POrderType { get; set; }

        public int ProductIdObjectType { get; set; }
    }
}
