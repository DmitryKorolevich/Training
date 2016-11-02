using System;
using System.Collections.Concurrent;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class EditLockPingModel
    {
        public string AreaName { get; set; }

        public int Id { get; set; }

        public int IdAgent { get; set; }

        public string Agent { get; set; }

        public string AgentFirstName { get; set; }

        public string AgentLastName { get; set; }
    }
}