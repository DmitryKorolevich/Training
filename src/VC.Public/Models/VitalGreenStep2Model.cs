using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Validators;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models
{
    [ApiValidator(typeof(VitalGreenRequestModelValidator))]
    public class VitalGreenRequestModel : BaseModel
    {
        [Display(Name = "First Name")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address { get; set; }

        [Display(Name = "Address2")]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address2 { get; set; }

        [Display(Name = "City")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required]
        public string StateCode { get; set; }

        [Display(Name = "Zip")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Zip { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [Required]
        [CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Phone { get; set; }

        public VitalGreenRequest Convert()
        {
            VitalGreenRequest toReturn = new VitalGreenRequest();
            toReturn.FirstName = FirstName;
            toReturn.LastName = LastName;
            toReturn.Address = Address;
            toReturn.Address2 = Address2;
            toReturn.City = City;
            toReturn.State = StateCode;
            toReturn.Zip = Zip;
            toReturn.Email = Email;
            toReturn.Phone = Phone;
            return toReturn;
        }
    }
}