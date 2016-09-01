using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Services;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Help
{
    public enum SendContentUrlType
    {
        Article=1,
        Recipe=2
    }

    public class SendContentUrlNotificationModel : BaseModel
    {
		[Display(Name = "Your Name")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [AllowXss]
        public string YourName { get; set; }

		[Display(Name = "Your Email")]
		[Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [EmailAddress]
		public string YourEmail { get; set; }

        [Display(Name = "Recipent Name")]
        [Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [AllowXss]
        public string RecipentName { get; set; }

        [Display(Name = "Recipent Email")]
        [Required, MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [EmailAddress]
        public string RecipentEmail { get; set; }

        [Display(Name = "Message")]
        [Required, MaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
        [AllowXss]
        public string Message { get; set; }

        [Display(Name = "Url")]
        [Required]
        public string Url { get; set; }

        [Display(Name = "Name")]
        [Required]
        [AllowXss]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public SendContentUrlType Type { get; set; }
    }
}
