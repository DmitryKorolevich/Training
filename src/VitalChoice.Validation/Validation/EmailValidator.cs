using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(model => model).EmailAddress().WithMessage("FieldNotValid", "EmailField", ValidationScope.Common).WithName("Email");
        }
    }
}