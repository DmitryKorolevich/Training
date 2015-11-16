using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.ContentPages
{
    public class ContentPageToContentCategory : Entity
    {
        public ContentPage ContentPage { get; set; }

        public int ContentPageId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}