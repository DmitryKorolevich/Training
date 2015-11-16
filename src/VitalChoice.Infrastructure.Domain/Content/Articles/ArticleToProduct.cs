using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Content.Articles
{
    public class ArticleToProduct : Entity
    {
        public int IdArticle { get; set; }

        public int IdProduct { get; set; }

        public ShortProductInfo ShortProductInfo { get; set; }
    }
}
