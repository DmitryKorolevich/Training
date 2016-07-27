using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class ProductQualitySkusReportItem : Entity
    {
        public int IdObjectType { get; set; }

        public int IdOrderSource { get; set; }

        public int OrderSourceIdObjectType { get; set; }

        public DateTime DateCreated { get; set; }

        public string LastName { get; set; }

        public string OrderNotes { get; set; }
    }
}
