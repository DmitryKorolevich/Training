using System;
using System.Linq;
using FluentValidation;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Public;
using VC.Admin.Validators.Customer;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Public
{
    public class EmailOrderManageModelValidator : ModelValidator<EmailOrderManageModel>
    {
        public override void Validate(EmailOrderManageModel value)
        {
            ValidationErrors.Clear();
            var addressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            var marketingPaymentModelValidator = ValidatorsFactory.GetValidator<MarketingModelRules>();
            var NCPaymentModelValidator = ValidatorsFactory.GetValidator<NCPaymentModelRules>();
            ParseResults(ValidatorsFactory.GetValidator<EmailOrderModelValidator>().Validate(value, ruleSet: "Main"));
            ParseResults(addressValidator.Validate(value.Shipping), "shipping");
            if (value.IdPaymentMethodType.HasValue)
            {
                if (value.IdPaymentMethodType.Value == (int)PaymentMethodType.Marketing && value.Marketing != null)//marketing
                {
                    ParseResults(marketingPaymentModelValidator.Validate(value.Marketing), "marketing");
                }
                if (value.IdPaymentMethodType.Value == (int)PaymentMethodType.NoCharge && value.NC != null)//nc
                {
                    ParseResults(NCPaymentModelValidator.Validate(value.NC), "nc");
                }
            }
        }

        private class EmailOrderModelValidator : AbstractValidator<EmailOrderManageModel>
        {
            public EmailOrderModelValidator()
            {
                RuleSet("Main",
                    () =>
                    {
                        RuleFor(model => model.IdPaymentMethodType)
                            .Must(p => p.HasValue)
                            .WithMessage(model => model.IdPaymentMethodType, ValidationMessages.FieldRequired);
                        RuleFor(model => model.IdRequestor)
                            .Must(p => p.HasValue)
                            .WithMessage(model => model.IdRequestor, ValidationMessages.FieldRequired);
                        RuleFor(model => model.IdReason)
                            .Must(p => p.HasValue)
                            .WithMessage(model => model.IdReason, ValidationMessages.FieldRequired);

                        RuleFor(model => model.DetailsOnEvent)
                            .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                            .WithMessage(model => model.DetailsOnEvent, ValidationMessages.FieldLength,
                                BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                        RuleFor(model => model.Instuction)
                            .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                            .WithMessage(model => model.Instuction, ValidationMessages.FieldLength,
                                BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                    });

            }
        }
    }
}