using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.TemplateModels
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
