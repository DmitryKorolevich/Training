using System;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public class ContentAreaListItemModel: BaseModel
    {
	    public int Id { get; set; }

	    public string Name { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

		public string AgentId { get; set; }
	}
}
