using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuAddressReportAddressItem
    {
        [Map]
        public string Company { get; set; }

        [Map]
        public string FirstName { get; set; }

        [Map]
        public string LastName { get; set; }

        [Map]
        public string Address1 { get; set; }

        [Map]
        public string Address2 { get; set; }

        [Map]
        public string City { get; set; }

        [Map]
        public int? IdCountry { get; set; }

        public string CountyCode { get; set; }

        [Map]
        public int? IdState { get; set; }

        public string StateCode { get; set; }

        [Map]
        public string County { get; set; }

        [Map]
        public string Zip { get; set; }

        [Map]
        public string Phone { get; set; }

        [Map]
        public string Fax { get; set; }
    }
}