using FluentValidation;
using VC.Admin.Models.Setting;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
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
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.STATE_COUNTRY_CODE_MAX_SIZE)
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldLength, BaseAppConstants.STATE_COUNTRY_CODE_MAX_SIZE);

                RuleFor(model => model.CountryName)
                    .NotEmpty()
                    .WithMessage(model => model.CountryName, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}