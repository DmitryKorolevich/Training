namespace VitalChoice.Infrastructure.Domain.Transfer.ContentManagement
{
    public class ArticleItemListFilter : FilterBase
    {
	    public string Name { get; set; }

        public int? CategoryId { get; set; }
    }
}