﻿using System;
using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;

namespace VC.Admin.Validators.Customer
{
    public class VCWellnessPaymentModelValidator : ModelValidator<VCWellnessEmployeeProgramPaymentModel>
    {
        public override void Validate(VCWellnessEmployeeProgramPaymentModel value)
        {
            ValidationErrors.Clear();
            var item = ValidatorsFactory.GetValidator<VCWellnessModelRules>();
            ParseResults(item.Validate(value));

            var billingAddressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            ParseResults(billingAddressValidator.Validate(value.Address));
        }
    }

    public class VCWellnessModelRules : AbstractValidator<VCWellnessEmployeeProgramPaymentModel>
    {
        public VCWellnessModelRules()
        {
            RuleFor(model => model.PaymentComment)
                .NotEmpty()
                .WithMessage(model => model.PaymentComment, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                .WithMessage(model => model.PaymentComment, ValidationMessages.FieldLength,
                    BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
        }
    }
}