using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    [ApiValidator(typeof(ContentCrossSellModelValidator))]
    public class ContentCrossSellModel : BaseModel
    {
		public ContentCrossSellType Type { get; set; }

		public ContentCrossSellModel()
	    {
			Items = new List<ContentCrossSellItemModel>();
			DefaultItems = new List<ContentCrossSellItemModel>();
		}

	    public IList<ContentCrossSellItemModel> Items { get; set; }

	    public IList<ContentCrossSellItemModel> DefaultItems { get; set; }
    }
}