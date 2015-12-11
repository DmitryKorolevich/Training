using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles
{
    public class TtlShortArticleListModel : PagedList<TtlShortArticleModel>
    {
		public string PreviousLink { get; set; }

        public string NextLink { get; set; }
    }
}
