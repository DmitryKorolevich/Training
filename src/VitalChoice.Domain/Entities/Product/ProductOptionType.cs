using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Product
{
    public class ProductOptionType : Entity
    {
        public string Name { get; set; }

        public ProductType? IdProductType { get; set; }

        public int? IdLookup { get; set; }

        public Lookup Lookup { get; set; }

        public int IdFieldType { get; set; }

        public string DefaultValue { get; set; }
    }
}
