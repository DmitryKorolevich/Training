using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class IconWidthValidator : AbstractValidator<int>
    {
        public IconWidthValidator()
        {
            RuleFor(model => model)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .WithMessage("FieldMin", "Width", ValidationScope.Common, 0)
                .LessThanOrEqualTo(64)
                .WithMessage("FieldMax", "Width", ValidationScope.Common, 64)
                .WithName("Width");
        }
    }
}