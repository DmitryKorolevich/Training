using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
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