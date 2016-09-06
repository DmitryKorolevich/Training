using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlCategorySkuModel
    {
        [Map]
        public int IdProduct { get; set; }

        [Map("Id")]
        public int IdSku { get; set; }

        [Map]
        public string Code { get; set; }

        public string Name { get; set; }

        public string SubTitle { get; set; }

        public string ShortDescription { get; set; }

        public string Url { get; set; }

        public string Thumbnail { get; set; }

        public decimal Price { get; set; }

        public bool InStock { get; set; }
    }
}
