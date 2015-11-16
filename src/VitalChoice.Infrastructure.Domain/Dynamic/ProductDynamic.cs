using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class ProductDynamic : MappedObject
    {
        public ProductDynamic()
        {
            Skus = new List<SkuDynamic>();
            CategoryIds = new List<int>();
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool Hidden { get; set; }

        public ICollection<SkuDynamic> Skus { get; set; }

        public ICollection<int> CategoryIds { get; set; }
    }
}
