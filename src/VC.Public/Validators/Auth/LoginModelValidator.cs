using FluentValidation;
using VitalChoice.Validation.Logic;
using VC.Public.Models.Auth;

namespace VC.Public.Validators.Auth
{
    public class LoginModelValidator : ModelValidator<LoginModel>
    {
        public override void Validate(LoginModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<LoginModelInnerValidator>().Validate(value));
        }

        private class LoginModelInnerValidator : AbstractValidator<LoginModel>
        {
            public LoginModelInnerValidator()
            { 
            //    RuleFor(model => model.Email)
            //        .Must(model => model.Length == 10)
            //        .WithMessage(model => model.Email, ValidationMessages.FieldRequired);
            }
        }
    }
}