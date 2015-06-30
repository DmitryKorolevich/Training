using System.Collections.Generic;
using System.Linq;
using NuGet;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class ProductMapped : MappedObject<Product, ProductOptionType, ProductOptionValue>
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public ProductType Type { get; set; }

        public bool Hidden { get; set; }

        public ICollection<SkuMapped> Skus { get; set; }

        public ICollection<int> CategoryIds { get; set; }
    }
}
