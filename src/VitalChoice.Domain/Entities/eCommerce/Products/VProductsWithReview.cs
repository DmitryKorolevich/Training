using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class VProductsWithReview : Entity
    {
        public int IdProduct { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string ProductName { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Rating { get; set; }
    }
}