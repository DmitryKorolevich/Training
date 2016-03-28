using FluentValidation;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Customer
{
    public class MarketingPaymentModelValidator : ModelValidator<MarketingPaymentModel>
    {
        public override void Validate(MarketingPaymentModel value)
        {
            ValidationErrors.Clear();
            var item = ValidatorsFactory.GetValidator<MarketingModelRules>();
            ParseResults(item.Validate(value));

            var billingAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(billingAddressValidator.Validate(value.Address));
        }
    }

    public class MarketingModelRules : AbstractValidator<MarketingPaymentModel>
    {
        public MarketingModelRules()
        {
            RuleFor(model => model.PaymentComment)
                .NotEmpty()
                .WithMessage(model => model.PaymentComment, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                .WithMessage(model => model.PaymentComment, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

            RuleFor(model => model.MarketingPromotionType)
                .Must(p => p.HasValue)
                .WithMessage(model => model.MarketingPromotionType, ValidationMessages.FieldRequired);
        }
    }
}
