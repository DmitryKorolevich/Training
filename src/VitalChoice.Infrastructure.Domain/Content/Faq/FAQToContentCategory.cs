using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Content.Faq
{
    public class FAQToContentCategory : Entity
    {
        public FAQ FAQ { get; set; }

        public int FAQId { get; set; }
        public ContentCategory ContentCategory { get; set; }

        public int ContentCategoryId { get; set; }
    }
}