using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Auth
{
    public class ForgotPasswordEmailModel : BaseModel
	{
		[Required]
		[EmailAddress]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string Email { get; set; }
	}
}
