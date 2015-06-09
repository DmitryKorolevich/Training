namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductToCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdProduct { get; set; }
    }
}
