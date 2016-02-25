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
    public class ContentCrossSellItemModel : BaseModel
    {
	    public int Id { get; set; }

	    public ContentCrossSellType Type { get; set; }

		public string Title { get; set; }

		public string ImageUrl { get; set; }

		public int IdSku { get; set; }

	    public string SkuCode { get; set; }

	    public decimal? WholesalePrice { get; set; }

	    public decimal? RetailPrice { get; set; }
    }
}