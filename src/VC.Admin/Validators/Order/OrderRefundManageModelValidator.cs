using System.Linq;
using FluentValidation;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Validation.Logic;
using VC.Admin.Validators.Customer;
using System;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VC.Admin.Models.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Validators.Order
{
    public class OrderRefundManageModelValidator : ModelValidator<OrderRefundManageModel>
    {
        public override void Validate(OrderRefundManageModel value)
        {
            ValidationErrors.Clear();
            var addressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(ValidatorsFactory.GetValidator<OrderRefundModelValidator>().Validate(value, ruleSet: "Main"));

            ParseResults(addressValidator.Validate(value.Shipping), "shipping");
            if (value.IdPaymentMethodType == (int)PaymentMethodType.Oac && value.Oac != null)
            {
                ParseResults(addressValidator.Validate(value.Oac.Address), "oac");
            }
        }

        private class OrderRefundModelValidator : AbstractValidator<OrderRefundManageModel>
        {
            public OrderRefundModelValidator()
            {
                RuleSet("Main",
                         () =>
                         {
                             RuleFor(model => model.IdPaymentMethodType)
                                .Must(p => p.HasValue)
                                .WithMessage(model => model.IdPaymentMethodType, ValidationMessages.FieldRequired);

                             RuleFor(model => model.OrderNotes)
                              .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                              .WithMessage(model => model.OrderNotes, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

                             RuleFor(model => model.Shipping)
                               .Must(p => p != null)
                               .WithMessage(model => model.Shipping, ValidationMessages.FieldRequired);
                             RuleFor(model => model.Oac)
                                .Must(p => p != null)
                                .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == (int)PaymentMethodType.Oac
                                    && p.OrderStatus != OrderStatus.OnHold)
                                .WithMessage(model => model.Oac, ValidationMessages.FieldRequired);
                         });
            }
        }
    }
}