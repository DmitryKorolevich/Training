using FluentValidation;
using VC.Admin.Models.Setting;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
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
                    .WithMessage(model => model.StateCode, ValidationMessages.FieldRequired);

                RuleFor(model => model.StateName)
                    .NotEmpty()
                    .WithMessage(model => model.StateName, ValidationMessages.FieldRequired);
            }
        }
    }
}