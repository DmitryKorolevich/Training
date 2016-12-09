﻿using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Profile
{
    public class ChangeProfileModel
	{
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "First Name")]
        [Map]
        public string FirstName { get; set; }

        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Last Name")]
        [Map]
        public string LastName { get; set; }

        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Company")]
        [Map]
        public string Company { get; set; }

        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Phone")]
        [Map]
        public string Phone { get; set; }

        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Fax")]
        [Map]
        public string Fax { get; set; }

        [EmailAddress]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "New Email")]
		public string NewEmail { get; set; }

		//[Required]
		[EmailAddress]
		[Compare("NewEmail")]
		[Display(Name = "Confirm Email")]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string ConfirmEmail { get; set; }

		[Display(Name = "Current Email")]
		[Map("Email")]
		public string CurrentEmail { get; set; }
	}
}
