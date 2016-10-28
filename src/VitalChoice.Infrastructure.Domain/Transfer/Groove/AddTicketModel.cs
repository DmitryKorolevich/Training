using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Groove
{
    public class AddTicketModel
    {
        public string body { get; set; }

        public string from { get; set; }

        public string to { get; set; }

        public string name { get; set; }

        public string subject { get; set; }
    }
}