using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs
{
    public class TtlFAQCategoryModel
    {
	    public TtlFAQCategoryModel()
	    {
			SubCategories = new List<TtlFAQCategoryModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

		public string Url { get; set; }

	    public IList<TtlFAQCategoryModel> SubCategories { get; set; }
    }
}
