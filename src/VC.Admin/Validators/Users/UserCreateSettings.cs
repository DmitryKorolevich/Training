using VitalChoice.Validators.Users;
using VitalChoice.Validation.Models;

namespace VC.Admin.Validators.Users
{
    public class UserCreateSettings : AbstractModeContainer<UserCreateMode>
    {
        public bool ShowBusinessCategories { get; set; }
        public bool ShowAdditionalInfo { get; set; }
        public bool ShowAccountType { get; set; }
    }
}