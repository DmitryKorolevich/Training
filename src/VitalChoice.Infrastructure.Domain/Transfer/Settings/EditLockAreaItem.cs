using System;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class EditLockAreaItem
    {
        public DateTime Expired { get; set; }

        public int? IdAgent { get; set; }

        public string Agent { get; set; }

        public string AgentFirstName { get; set; }

        public string AgentLastName { get; set; }

        public string BrowserUserAgent { get; set; }


        public string LockMessageTitle { get; set; }

        public string LockMessageBody { get; set; }
    }
}