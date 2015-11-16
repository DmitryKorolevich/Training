using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Recipes
{
    public class RecipeToContentCategory : Entity
    {
        public Recipe Recipe { get; set; }

        public int RecipeId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}