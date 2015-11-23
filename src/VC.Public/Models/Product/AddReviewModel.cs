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
    public class AddReviewModel:BaseModel
    {
		[Display(Name = "Customer Name")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string CustomerName { get; set; }

		[Display(Name = "Review Title")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string ReviewTitle { get; set; }

		[Required, Range(1,5)]
	    public int Rating { get; set; }

		[Required, MaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
		public string Review { get; set; }

		[Required, EmailAddress, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string Email { get; set; }

		public Guid ProductId { get; set; }
    }
}
