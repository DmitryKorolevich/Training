using Microsoft.AspNet.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Help
{
    public class CustomerServiceRequestModel : BaseModel
    {
        [Required]
        public CustomerServiceRequestType Type { get; set; }

        public ICollection<SelectListItem> Types { get; set; }

        [Required]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Name")]
        public string Name {get;set;}

        [Required]
        [EmailAddress]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
		[MaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Comment")]
		public string Comment { get; set; }
    }

    public enum CustomerServiceRequestType
    {
        CustomerService=1,
        Feedback=2,
    }
}
