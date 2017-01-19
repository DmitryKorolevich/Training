using System;
using System.Linq;
using FluentValidation;
using VC.Admin.Models.Customers;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Public;
using VC.Admin.Validators.Customer;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Domain.Transfer.Public;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Public
{
    public class EmailOrderManageModelValidator : ModelValidator<EmailOrderManageModel>
    {
        public override void Validate(EmailOrderManageModel value)
        {
            ValidationErrors.Clear();
            var addressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            var shippingWillCallAddressModelRules = ValidatorsFactory.GetValidator<ShippingWillCallAddressModelRules>();
            ParseResults(ValidatorsFactory.GetValidator<EmailOrderModelValidator>().Validate(value, ruleSet: "Main"));
            if (value.IdEmailOrderShippingType.HasValue)
            {
                if (value.IdEmailOrderShippingType.Value == (int) EmailOrderShippingType.WillCall)
                {
                    ParseResults(shippingWillCallAddressModelRules.Validate(value.Shipping), "shipping");
                }
                else
                {
                    ParseResults(addressValidator.Validate(value.Shipping), "shipping");
                }
            }
        }

        public class ShippingWillCallAddressModelRules : AbstractValidator<AddressModel>
        {
            public ShippingWillCallAddressModelRules()
            {
                RuleFor(model => model.Company)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Company, ValidationMessages.FieldLength,
                        BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.FirstName)
                    .NotEmpty()
                    .WithMessage(model => model.FirstName, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.FirstName, ValidationMessages.FieldLength,
                        BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.LastName)
                    .NotEmpty()
                    .WithMessage(model => model.LastName, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.LastName, ValidationMessages.FieldLength,
                        BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
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