using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class LatitudeValidator : AbstractValidator<double>
    {
        public LatitudeValidator()
        {
            RuleFor(model => model)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(-90)
                .WithMessage("FieldNotValid", "Latitude", ValidationScope.Api)
                .LessThanOrEqualTo(90)
                .WithMessage("FieldNotValid", "Latitude", ValidationScope.Api)
                .WithName("Latitude");
        }
    }
}