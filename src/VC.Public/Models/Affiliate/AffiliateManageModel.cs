using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Validators.Affiliate;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Affiliate
{
    [ApiValidator(typeof(AffiliateManageModelValidator))]
    public class AffiliateManageModel : BaseModel
	{
        [Display(Name = "Current Email")]
        public string CurrentEmail { get; set; }

        [EmailAddress]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Email")]
        [Map]
        public string Email { get; set; }

        [EmailAddress]
        [Compare("Email")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Email Confirm")]
        public string ConfirmEmail { get; set; }

        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Password { get; set; }

		[Compare("Password")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Password Confirm")]
		public string ConfirmPassword { get; set; }

        
        [Display(Name = "Website")]
        [Map]
        public bool PromoteByWebsite { get; set; }

        [Display(Name = "Website URL")]
        [Map]
        public string WebSite { get; set; }

        [Display(Name = "Links on your Emails")]
        [Map]
        public bool PromoteByEmails { get; set; }

        [Display(Name = "Monthly Emails sent")]
        [Map]
        public int? MonthlyEmailsSent { get; set; }

        public IList<SelectListItem> MonthlyEmailsSentOptions { get; set; }

        [Display(Name = "Facebook")]
        [Map]
        public bool PromoteByFacebook { get; set; }

        [Display(Name = "Facebook page URL")]
        [Map]
        public string Facebook { get; set; }

        [Display(Name = "Twitter")]
        [Map]
        public bool PromoteByTwitter { get; set; }

        [Display(Name = "Twitter page URL")]
        [Map]
        public string Twitter { get; set; }

        [Display(Name = "Blog")]
        [Map]
        public bool PromoteByBlog { get; set; }

        [Display(Name = "Blog site URL")]
        [Map]
        public string Blog { get; set; }

        [Display(Name = "Professional practice")]
        [Map]
        public bool PromoteByProfessionalPractice { get; set; }

        [Display(Name = "Practice")]
        [Map]
        public int? ProfessionalPractice { get; set; }

        public IList<SelectListItem> ProfessionalPracticeOptions { get; set; }

        [Map]
        public bool PromoteByDrSearsLEANCoachAmbassador { get; set; }

        [Map]
        public bool PromoteByVerticalResponseEmail { get; set; }

        [Map]
        public bool PromoteByLeanEmail { get; set; }


        [Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Name")]
		[Map]
		public string Name { get; set; }

        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Checks payable to")]
        [Map]
        public string ChecksPayableTo { get; set; }

        [Required]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Tax ID / SSN")]
        [Map]
        public string TaxID { get; set; }

        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Company")]
		[Map]
		public string Company { get; set; }

		[Required]
		[Display(Name = "Country")]
		[Map]
		public int IdCountry { get; set; }

        [Display(Name = "State/Province")] //required if
		[Map]
		public int IdState { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Address")]
		[Map]
		public string Address1 { get; set; }

		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string Address2 { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "City")]
		[Map]
		public string City { get; set; }

		[Display(Name = "State/Province")]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string County { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Postal Code")]
		[Map("Zip")]
		public string PostalCode { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Phone")]
		[Map]
		public string Phone { get; set; }

		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Fax")]
		[Map]
		public string Fax { get; set; }

        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "How did you hear about Vital Choice?")]
        [Map]
        public string HearAbout { get; set; }

        [Display(Name = "I would like payment as")]
        [Map]
        public int PaymentType { get; set; }

        [Map]
        public decimal CommissionAll { get; set; }

        [Map]
        public decimal CommissionFirst { get; set; }

        public bool IsNotSpam { get; set; }
        
        public bool IsAllowAgreement { get; set; }

        public AffiliateDynamic Update(AffiliateDynamic item)
        {
            if (item != null)
            {
                if (!String.IsNullOrEmpty(Email))
                {
                    item.Email = Email;
                }
                item.Name = Name;
                item.Data.ChecksPayableTo = ChecksPayableTo;
                item.Data.TaxID = TaxID;
                item.Data.Company = Company;
                item.IdCountry = IdCountry;
                item.IdState = IdState;
                item.County = County;
                item.Data.Address1 = Address1;
                item.Data.Address2 = Address2;
                item.Data.City = City;
                item.Data.Zip = PostalCode;
                item.Data.Phone = Phone;
                item.Data.Fax = Fax;
                item.Data.PaymentType = PaymentType;
            }
            return item;
        }
    }
}
