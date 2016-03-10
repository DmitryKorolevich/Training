using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates
{
    public class GeneratedGiftCertificate
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal Balance { get; set; }

        public SkuDynamic Sku { get; set; }

        public ProductDynamic Product { get; set; }
    }
}
