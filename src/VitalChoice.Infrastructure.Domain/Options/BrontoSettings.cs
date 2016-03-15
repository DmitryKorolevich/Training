using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class BrontoSettings
    {
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public string PublicFormUrl { get; set; }
        public string PublicFormSendData { get; set; }
        public string PublicFormSubscribeData { get; set; }
    }
}