using FluentValidation;
using VC.Admin.Models.InventorySkus;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;

namespace VC.Admin.Validators.InventorySku
{
    public class InventorySkuManageModelValidator : ModelValidator<InventorySkuManageModel>
    {
        public override void Validate(InventorySkuManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<InventorySkuModelValidator>().Validate(value));
        }

        private class InventorySkuModelValidator : AbstractValidator<InventorySkuManageModel>
        {
            public InventorySkuModelValidator()
            {
                RuleFor(model => model.Code)
                    .NotEmpty()
                    .WithMessage(model => model.Code, ValidationMessages.FieldRequired)
                    .Matches("^[A-Za-z0-9-]*")
                    .WithMessage(model => model.Code, ValidationMessages.FieldInvalid)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Code, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Description, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}