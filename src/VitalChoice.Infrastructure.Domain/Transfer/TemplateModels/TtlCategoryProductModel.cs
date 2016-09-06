using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlCategoryProductModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public string Name { get; set; }

        [Map]
        public string SubTitle { get; set; }

        [Map]
        public string Url { get; set; }

        [Map]
        public string Thumbnail { get; set; }
	}
}
