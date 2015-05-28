﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.Product
{
    public class VProductSku: Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int IdProduct { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public string Url { get; set; }

        public string Thumbnail { get; set; }
    }
}
