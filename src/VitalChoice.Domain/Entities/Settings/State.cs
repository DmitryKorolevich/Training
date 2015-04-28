using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Settings
{
    public class State : Entity
    {
        public string StateCode { get; set; }

        public string CountryCode { get; set; }

        public string StateName { get; set; }

        public int Order { get; set; }

        public Content.RecordStatusCode StatusCode { get; set; }
    }
}
