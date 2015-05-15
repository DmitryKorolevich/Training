using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.Product
{
    public class GiftCertificateEmail : Entity
    {
        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string FromName { get; set; }

        public string Message { get; set; }
    }
}