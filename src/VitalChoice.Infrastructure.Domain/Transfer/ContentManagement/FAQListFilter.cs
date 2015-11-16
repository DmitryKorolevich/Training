namespace VitalChoice.Infrastructure.Domain.Transfer.ContentManagement
{
    public class FAQListFilter : FilterBase
    {
	    public string Name { get; set; }

        public int? CategoryId { get; set; }
    }
}