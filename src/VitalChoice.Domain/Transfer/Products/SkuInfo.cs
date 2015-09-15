using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Transfer.Products
{
    public class SkuInfo
    {
        public int Id { get; set; }

        public ProductType IdProductType { get; set; }
    }
}
