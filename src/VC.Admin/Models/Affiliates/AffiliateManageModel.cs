using System;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Affiliate;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VC.Admin.Models.Affiliates
{
    [ApiValidator(typeof(AffiliatetManageModelValidator))]
    public class AffiliateManageModel : BaseModel
    {
        public bool IsConfirmed { get; set; }

        public Guid PublicUserId { get; set; }

        [Map]
        public int Id { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public string SuspendMessage { get; set; }

        [Map]
        public string Name { get; set; }

        [Map]
        public decimal MyAppBalance { get; set; }
        
        [Map]
        public decimal CommissionFirst { get; set; }

        [Map]
        public decimal CommissionAll { get; set; }

        [Map]
        public int? IdCountry { get; set; }

        public string CountryCode { get; set; }

        [Map]
        public int? IdState { get; set; }

        public string StateCode { get; set; }

        [Map]
        public string County { get; set; }

        [Map]
        public DateTime DateEdited { get; set; }


        [Map]
        public bool BrickAndMortar { get; set; }

        [Map]
        public bool PromoteByWebsite { get; set; }

        [Map]
        public bool PromoteByEmails { get; set; }

        [Map]
        public int? MonthlyEmailsSent { get; set; }

        [Map]
        public bool PromoteByFacebook { get; set; }

        [Map]
        public string Facebook { get; set; }

        [Map]
        public bool PromoteByTwitter { get; set; }

        [Map]
        public string Twitter { get; set; }

        [Map]
        public bool PromoteByBlog { get; set; }

        [Map]
        public string Blog { get; set; }

        [Map]
        public bool PromoteByProfessionalPractice { get; set; }

        [Map]
        public int? ProfessionalPractice { get; set; }

        [Map]
        public bool PromoteByDrSearsLEANCoachAmbassador { get; set; }

        [Map]
        public bool PromoteByVerticalResponseEmail { get; set; }

        [Map]
        public bool PromoteByLeanEmail { get; set; }

        [Map]
        public string ChecksPayableTo { get; set; }

        [Map]
        public string Email { get; set; }

        [Map]
        public string EmailConfirm { get; set; }

        [Map]
        public string TaxID { get; set; }

        [Map]
        public string Company { get; set; }

        [Map]
        public string Phone { get; set; }

        [Map]
        public string Fax { get; set; }

        [Map]
        public string Address1 { get; set; }

        [Map]
        public string Address2 { get; set; }

        [Map]
        public string City { get; set; }

        [Map]
        public string Zip { get; set; }

        [Map]
        public string HearAbout { get; set; }

        [Map]
        public string WebSite { get; set; }

        [Map]
        public string Reach { get; set; }

        [Map]
        public string Profession { get; set; }

        [Map]
        public int Tier { get; set; }

        [Map]
        public string Notes { get; set; }

        [Map]
        public int PaymentType { get; set; }


        public AffiliateManageModel()
        {

        }
    }
}