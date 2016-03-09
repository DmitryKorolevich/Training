using FluentValidation;
using VC.Admin.Models.InventorySkus;
using VC.Admin.Models.Products;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.InventorySku
{
    public class InventorySkuCategoryManageModelValidator : ModelValidator<InventorySkuCategoryManageModel>
    {
        public override void Validate(InventorySkuCategoryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<InventorySkuCategoryModelValidator>().Validate(value));
        }

        private class InventorySkuCategoryModelValidator : AbstractValidator<InventorySkuCategoryManageModel>
        {
            public InventorySkuCategoryModelValidator()
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