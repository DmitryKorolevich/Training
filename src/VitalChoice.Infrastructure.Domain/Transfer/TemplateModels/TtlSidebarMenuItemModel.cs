using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.TemplateModels
{
    public class TtlSidebarMenuItemModel
    {
		public string Label { get; set; }

		public string Url { get; set; }

		public IList<TtlSidebarMenuItemModel> SubItems { get; set; }

		public TtlSidebarMenuItemModel()
		{
			SubItems = new List<TtlSidebarMenuItemModel>();
		}
	}
}
