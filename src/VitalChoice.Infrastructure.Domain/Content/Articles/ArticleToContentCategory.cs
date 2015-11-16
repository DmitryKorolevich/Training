using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Articles
{
    public class ArticleToContentCategory : Entity
    {
        public Article Article { get; set; }

        public int ArticleId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}