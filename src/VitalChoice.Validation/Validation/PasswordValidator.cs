using System.Linq;
using FluentValidation;
using VitalChoice.Validation.Validation.Interfaces;

namespace VitalChoice.Validation.Validation
{
    public class PasswordValidator : AbstractValidator<IPasswordModel>
    {
        public PasswordValidator()
        {
            RuleFor(model => model.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Equal(model => model.Password2)
                .WithMessage("PasswordsMatch", ValidationScope.Common)
                .Length(6, int.MaxValue)
                .WithMessage("FieldMinimumLength", "PasswordField", ValidationScope.Common, 6)
                .Must
                (model => !model.Any(char.IsControl) && !model.Any(char.IsPunctuation) && !model.Any(char.IsWhiteSpace))
                .WithMessage("PasswordRules", ValidationScope.Common, 6);
        }
    }
}