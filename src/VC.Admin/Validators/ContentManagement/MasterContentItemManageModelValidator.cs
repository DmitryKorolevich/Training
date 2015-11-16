using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.ContentManagement
{
    public class MasterContentItemManageModelValidator : ModelValidator<MasterContentItemManageModel>
    {
        public override void Validate(MasterContentItemManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<MasterContentItemModelValidator>().Validate(value));
        }

        private class MasterContentItemModelValidator : AbstractValidator<MasterContentItemManageModel>
        {
            public MasterContentItemModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);
            }
        }
    }
}