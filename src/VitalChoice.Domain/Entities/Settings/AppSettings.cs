using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Settings
{
    public class AppSettings
    {
        public int? GlobalPerishableThreshold { get; set; }

        public bool CreditCardAuthorizations { get; set; }
    }
}
