using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class Sku : DynamicDataEntity<ProductOptionValue, ProductOptionType>
    {
        public int IdProduct { get; set; }

        public string Code { get; set; }

        public bool Hidden { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }
    }
}
