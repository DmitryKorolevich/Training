using FluentValidation;
using VC.Admin.Models.Setting;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Models.Setting;

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
                    .WithMessage(model => model.StateCode, ValidationMessages.FieldRequired);

                RuleFor(model => model.StateName)
                    .NotEmpty()
                    .WithMessage(model => model.StateName, ValidationMessages.FieldRequired);
            }
        }
    }
}