using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.Content
{
    public class ArticleToProduct : Entity
    {
        public int IdArticle { get; set; }

        public int IdProduct { get; set; }

        public ShortProductInfo ShortProductInfo { get; set; }
    }
}
