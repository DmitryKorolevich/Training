using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VitalChoice.Validators.Users
{
    public class UserCreateAdminValidator: AbstractValidator<UserCreateModel>
    {
        public UserCreateAdminValidator()
        {
            RuleFor(model => model.Name).NotEmpty().WithMessage(model => model.Name, ValidationMessages.FieldRequired);
        }
    }
}