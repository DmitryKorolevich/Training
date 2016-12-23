using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class CustomerAbuseReportItem
    {
        public int IdCustomer { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int TotalOrders { get; set; }

        public int TotalReships { get; set; }

        public int TotalRefunds { get; set; }

        public DateTime? LastOrderDateCreated { get; set; }

        public string ServiceCodes { get; set; }
    }
}