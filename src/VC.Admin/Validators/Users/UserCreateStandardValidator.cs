using FluentValidation;
using VC.Admin.Models;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Users
{
    public class UserCreateStandardValidator : AbstractValidator<UserCreateModel>
    {
        public UserCreateStandardValidator()
        {
            RuleSet
                ("Main",
                 () =>
                     {
                         var data2 = LocalizedMessages.Do();
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