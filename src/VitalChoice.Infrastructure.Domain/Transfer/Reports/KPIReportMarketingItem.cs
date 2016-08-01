using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class KPIReportMarketingItem
    {
        public long UniqueVisits { get; set; }

        public decimal NewWebVisitsPercent { get; set; }

        public long NewEmailAddresses { get; set; }

        public decimal OpenRate { get; set; }

        public long FacebookLikes { get; set; }

        public long TwitterFollowers { get; set; }
    }
}