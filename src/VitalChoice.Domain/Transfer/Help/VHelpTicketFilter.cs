using System;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Help
{
    public class VHelpTicketFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public RecordStatusCode? StatusCode { get; set; }
    }
}