using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class AAFESReportItem : Entity
    {
        public long RowNumber { get; set; }

        public int IdOrder { get; set; }

        public int IdSku { get; set; }

        public string OrderNotes { get; set; }

        public string ShipMethodFreightCarrier { get; set; }

        public string TrackingNumber { get; set; }

        public string Code { get; set; }

        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ShippedDate { get; set; }

        public string ServiceUrl { get; set; }
    }
}
