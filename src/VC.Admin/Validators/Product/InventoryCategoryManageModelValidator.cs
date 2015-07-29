using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Domain.Constants;

namespace VC.Admin.Validators.Product
{
    public class InventoryCategoryManageModelValidator : ModelValidator<InventoryCategoryManageModel>
    {
        public override void Validate(InventoryCategoryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<InventoryCategoryModelValidator>().Validate(value));
        }

        private class InventoryCategoryModelValidator : AbstractValidator<InventoryCategoryManageModel>
        {
            public InventoryCategoryModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}