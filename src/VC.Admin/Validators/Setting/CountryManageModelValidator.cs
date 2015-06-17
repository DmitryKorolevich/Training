using FluentValidation;
using VC.Admin.Models.Setting;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Setting
{
    public class CountryManageModelValidator : ModelValidator<CountryManageModel>
    {
        public override void Validate(CountryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<CountryModelValidator>().Validate(value));
        }

        private class CountryModelValidator : AbstractValidator<CountryManageModel>
        {
            public CountryModelValidator()
            {
                RuleFor(model => model.CountryCode)
                    .NotEmpty()
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldRequired);

                RuleFor(model => model.CountryName)
                    .NotEmpty()
                    .WithMessage(model => model.CountryName, ValidationMessages.FieldRequired);
            }
        }
    }
}