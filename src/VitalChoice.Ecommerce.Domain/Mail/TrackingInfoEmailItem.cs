using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class TrackingInfoEmailItem
    {
        public string Number { get; set; }

        public string ServiceUrl { get; set; }
    }
}
