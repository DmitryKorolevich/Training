using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Content.Recipes
{
    public class RecipeToProduct : Entity
    {
        public int IdRecipe { get; set; }

        public int IdProduct { get; set; }

        public ShortProductInfo ShortProductInfo { get; set; }
    }
}
