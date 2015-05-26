using System.Linq;
using FluentValidation;
using VitalChoice.Validation.Logic.Interfaces;

namespace VitalChoice.Validation.Logic
{
    public class PasswordValidator : AbstractValidator<IPasswordModel>
    {
        //public PasswordValidator()
        //{
        //    RuleFor(model => model.Password)
        //        .Cascade(CascadeMode.StopOnFirstFailure)
        //        .Equal(model => model.Password2)
        //        .WithMessageWithParams("PasswordsMatch")
        //        .Length(6, int.MaxValue)
        //        .WithMessage("FieldMinimumLength", "PasswordField", 6)
        //        .Must
        //        (model => !model.Any(char.IsControl) && !model.Any((c) => c == '.') && !model.Any((c) => c == ',') && !model.Any(char.IsWhiteSpace))
        //        .WithMessageWithParams("PasswordRules",6);
        //}
    }
}