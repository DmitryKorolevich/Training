using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Users;
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
