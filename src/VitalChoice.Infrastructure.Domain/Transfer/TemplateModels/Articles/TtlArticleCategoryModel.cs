using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles
{
    public class TtlShortArticleModel
    {
	    public TtlShortArticleModel()
	    {
        }

	    public string Name { get; set; }

		public string Url { get; set; }

        public DateTime? PublishedDate { get; set; }
    }
}
