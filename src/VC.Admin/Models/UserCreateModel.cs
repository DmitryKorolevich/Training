using VC.Admin.Validators.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Validation.Attributes;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models
{
    [ApiValidator(typeof(UserCreateValidator))]
    public class UserCreateModel : Model<User, UserCreateSettings>
    {
        [Localized(BaseButtonLabels.Go)]
        public string Name { get; set; }

        [Localized(BaseButtonLabels.Cancel)]
        public int? AccountTypeId { get; set; }

        public UserCreateModel()
        {

        }

        public override User Convert()
        {
            User user = new User
            {
                Name = Name,
                AccountTypeId = AccountTypeId,
            };
            return user;
        }
    }
}
