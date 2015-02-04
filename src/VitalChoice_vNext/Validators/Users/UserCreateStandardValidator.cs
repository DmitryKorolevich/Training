using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Models;
using VitalChoice.Validation.Logic;

namespace VitalChoice.Validators.Users
{
    public class UserCreateStandardValidator : AbstractValidator<UserCreateModel>
    {
        public UserCreateStandardValidator()
        {
            RuleSet
                ("Main",
                 () =>
                     {
                         RuleFor(model => model.Name)
                             .NotEmpty()
                             .WithMessage(model => model.Name, ValidationMessages.FieldRequired);
                     });
            RuleSet("WithAccountType",
                    () =>
                         RuleFor(model => model.AccountTypeId)
                             .NotNull()
                             .WithMessage(model => model.AccountTypeId, ValidationMessages.FieldRequired));
        }
    }
}