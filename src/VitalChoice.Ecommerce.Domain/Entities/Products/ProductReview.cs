using System;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class ProductReview : Entity
    {
        public RecordStatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public int IdProduct { get; set; }

        public Product Product { get; set; }

        public string CustomerName { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }
    }
}