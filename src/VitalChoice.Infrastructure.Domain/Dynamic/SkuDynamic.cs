﻿using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class SkuDynamic : MappedObject
    {
        public string Code { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Order { get; set; }

        public int IdProduct { get; set; }
    }
}