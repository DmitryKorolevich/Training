using FluentValidation;
using VC.Admin.Models.Setting;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Setting
{
    public class StateManageModelValidator : ModelValidator<StateManageModel>
    {
        public override void Validate(StateManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<StateModelValidator>().Validate(value));
        }

        private class StateModelValidator : AbstractValidator<StateManageModel>
        {
            public StateModelValidator()
            {
                RuleFor(model => model.StateCode)
                    .NotEmpty()
                    .WithMessage(model => model.StateCode, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.STATE_COUNTRY_CODE_MAX_SIZE)
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldLength, BaseAppConstants.STATE_COUNTRY_CODE_MAX_SIZE);

                RuleFor(model => model.StateName)
                    .NotEmpty()
                    .WithMessage(model => model.StateName, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}