using FluentValidation;

namespace VitalChoice.Validation.Validation
{
    public class LongitudeValidator : AbstractValidator<double>
    {
        public LongitudeValidator()
        {
            RuleFor(model => model)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(-180.0)
                .WithMessage("FieldNotValid", "Longitude", ValidationScope.Api)
                .LessThanOrEqualTo(180.0)
                .WithMessage("FieldNotValid", "Longitude", ValidationScope.Api)
                .WithName("Longitude");
        }
    }
}