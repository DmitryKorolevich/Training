using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.Content
{
    public class RecipeToProduct : Entity
    {
        public int IdRecipe { get; set; }

        public int IdProduct { get; set; }

        public ShortProductInfo ShortProductInfo { get; set; }
    }
}
