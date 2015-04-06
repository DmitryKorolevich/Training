using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VitalChoice.Validators.ContentManagement
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