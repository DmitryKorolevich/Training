using System;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class EditLockAreaItem
    {
        public DateTime Expired { get; set; }

        public int IdAgent { get; set; }

        public string Agent { get; set; }

        public string BrowserUserAgent { get; set; }
    }
}