﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.VitalGreen
{
    public class VitalGreenDropoffLocation
    {
        public string BusinessName { get; set; }

        public string Distance { get; set; }

        public string FedExAddress { get; set; }

        public string City { get; set; }

        public string FedExState { get; set; }

        public string FedExZip { get; set; }

        public string Weekdays { get; set; }

        public string Saturdays { get; set; }
    }
}
