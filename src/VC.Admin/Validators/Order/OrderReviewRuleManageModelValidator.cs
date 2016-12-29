using FluentValidation;
using VC.Admin.Models.Orders;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Order
{
    public class OrderReviewRuleManageModelValidator : ModelValidator<OrderReviewRuleManageModel>
    {
        public override void Validate(OrderReviewRuleManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<OrderReviewRuleModelValidator>().Validate(value));
        }

        private class OrderReviewRuleModelValidator : AbstractValidator<OrderReviewRuleManageModel>
        {
            public OrderReviewRuleModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.DeliveryInstructionForSearch)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.DeliveryInstructionForSearch, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.ZipForSearch)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ZipForSearch, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.SkuForSearch)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SkuForSearch, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.CompareNamesType)
                   .Must(p => p.HasValue)
                   .When(p=>p.CompareNames)
                   .WithMessage(model => model.CompareNamesType, ValidationMessages.FieldRequired);

                RuleFor(model => model.CompareAddressesType)
                   .Must(p => p.HasValue)
                   .When(p => p.CompareAddresses)
                   .WithMessage(model => model.CompareAddressesType, ValidationMessages.FieldRequired);

                RuleFor(model => model.ReshipsRefundsCheckType)
                   .Must(p => p.HasValue)
                   .When(p => p.ReshipsRefundsCheck)
                   .WithMessage(model => model.ReshipsRefundsCheckType, ValidationMessages.FieldRequired);

                RuleFor(model => model.ReshipsRefundsQTY)
                   .Must(p => p.HasValue)
                   .When(p => p.ReshipsRefundsCheck)
                   .WithMessage(model => model.ReshipsRefundsQTY, ValidationMessages.FieldRequired);

                RuleFor(model => model.ReshipsRefundsMonthCount)
                   .Must(p => p.HasValue)
                   .When(p => p.ReshipsRefundsCheck)
                   .WithMessage(model => model.ReshipsRefundsMonthCount, ValidationMessages.FieldRequired);
            }
        }
    }
}