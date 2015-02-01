using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class DomainNameValidator : AbstractValidator<string>
    {
        public DomainNameValidator()
        {
            RuleFor(model => model)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, 50)
                .WithMessage("FieldLength", "DomainName", ValidationScope.Common, 50)
                .Matches(ValidationPatterns.DomainPattern)
                .WithMessage("DomainNameFormat", "DomainName", ValidationScope.Common)
                .WithName("DomainName");
        }
    }
}