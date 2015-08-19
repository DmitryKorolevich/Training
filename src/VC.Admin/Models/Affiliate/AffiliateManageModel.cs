using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Interfaces;
using VC.Admin.Validators.Affiliate;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(AffiliatetManageModelValidator))]
    public class AffiliateManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

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

        [Map]
        public int? IdState { get; set; }

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

        [Map]
        [Localized(GeneralFieldNames.Password)]
        public string Password { get; set; }

        [Map]
        [Localized(GeneralFieldNames.ConfirmPassword)]
        public string ConfirmPassword { get; set; }


        public AffiliateManageModel()
        {

        }
    }
}