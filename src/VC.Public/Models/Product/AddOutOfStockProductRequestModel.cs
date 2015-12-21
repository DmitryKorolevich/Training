using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Services;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Product
{
    public class AddOutOfStockProductRequestModel : BaseModel
    {
		[Display(Name = "Name")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string Name { get; set; }

		[Display(Name = "Email")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [EmailAddress]
		public string Email { get; set; }

		public Guid ProductId { get; set; }
    }
}
