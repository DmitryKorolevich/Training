using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.ContentManagement
{
    public class ArticleItemListFilter : FilterBase
    {
	    public string Name { get; set; }

        public int? CategoryId { get; set; }

        public ICollection<int> ExcludeIds { get; set; }
    }
}