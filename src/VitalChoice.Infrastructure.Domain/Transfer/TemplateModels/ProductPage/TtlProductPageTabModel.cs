using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.ProductPage
{
    public class TtlProductPageTabModel
    {
        public virtual string TitleOverride { get; set; }

	    public virtual string Content { get; set; }

	    public virtual bool Hidden { get; set; }
    }
}
