using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Help
{
    public class PrivacyRequestModel : BaseModel
    {
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Your Name")]
        public string Name {get;set;}

		[Required]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Mailing Address")]
		public string MailingAddress { get; set; }

		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Other Name")]
		[Map]
		public string OtherName { get; set; }

		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Other Address")]
		[Map]
		public string OtherAddress { get; set; }

        [CustomMaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
    }
}
