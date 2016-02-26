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
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Validators.Order
{
    public class OrderManualSendConfirmationModelValidator : ModelValidator<OrderManualSendConfirmationModel>
    {
        public override void Validate(OrderManualSendConfirmationModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<OrderManualSendConfirmationModelInnerValidator>().Validate(value));
        }

        private class OrderManualSendConfirmationModelInnerValidator : AbstractValidator<OrderManualSendConfirmationModel>
        {
            public OrderManualSendConfirmationModelInnerValidator()
            {
                RuleFor(model => model.Email)
                  .NotEmpty()
                  .WithMessage(model => model.Email, ValidationMessages.FieldRequired)
                  .EmailAddress()
                  .WithMessage(model => model.Email, ValidationMessages.EmailFormat)
                  .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                  .WithMessage(model => model.Email, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}