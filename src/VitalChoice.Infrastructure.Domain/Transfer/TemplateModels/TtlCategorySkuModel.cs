namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlCategorySkuModel
    {
        public int IdProduct { get; set; }

        public int IdSku { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string SubTitle { get; set; }

        public string ShortDescription { get; set; }

        public string Url { get; set; }

		public string Thumbnail { get; set; }

        public decimal Price { get; set; }
    }
}
