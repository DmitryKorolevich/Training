﻿using System.Linq;
using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VC.Admin.Validators.Customer
{
	public class CustomerAddUpdateModelValidator : ModelValidator<AddUpdateCustomerModel>
	{
        public override void Validate(AddUpdateCustomerModel value)
	    {
	        ValidationErrors.Clear();

	        var customerValidator = ValidatorsFactory.GetValidator<CustomerModelRules>();
	        ParseResults(customerValidator.Validate(value));

	        var profileAddressValidator = ValidatorsFactory.GetValidator<ProfileAddressModelRules>();
	        ParseResults(profileAddressValidator.Validate(value.ProfileAddress), "profile");

	        int index = 0;
            var addressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            foreach (var shipping in value.Shipping)
	        {
	            ParseResults(addressValidator.Validate(shipping), "Shipping", index, "shipping");
	            index++;
	        }

	        var creditCardValidator = ValidatorsFactory.GetValidator<CreditCardModelRules>();
	        index = 0;
            foreach (var creditCard in value.CreditCards)
	        {
	            ParseResults(creditCardValidator.Validate(creditCard), "CreditCards", index, "card");
                ParseResults(addressValidator.Validate(creditCard.Address), "CreditCards", index, "card");
	            index++;
	        }

            if (value.Check != null)
            {
                var checkPaymentModelValidator = ValidatorsFactory.GetValidator<CheckPaymentModelRules>();
                ParseResults(addressValidator.Validate(value.Check.Address), "check");
            }

            if (value.Oac != null)
            {
                var oacPaymentModelValidator = ValidatorsFactory.GetValidator<OacPaymentModelRules>();
                ParseResults(oacPaymentModelValidator.Validate(value.Oac), "oac");
                ParseResults(addressValidator.Validate(value.Oac.Address), "oac");
            }

            if (value.WireTransfer != null)
            {
                ParseResults(addressValidator.Validate(value.WireTransfer.Address), "wiretransfer");
            }

            if (value.Marketing != null)
            {
                ParseResults(addressValidator.Validate(value.Marketing.Address), "marketing");
            }

            if (value.VCWellness != null)
            {
                ParseResults(addressValidator.Validate(value.VCWellness.Address), "vcwellness");
            }

            if (value.NC != null)
            {
                ParseResults(addressValidator.Validate(value.NC.Address), "nc");
            }
        }

	    public class CustomerModelRules : AbstractValidator<AddUpdateCustomerModel>
		{
			public CustomerModelRules()
			{
				RuleFor(model => model.Website)
					.NotEmpty()
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Website, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Website, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

				RuleFor(model => model.PromotingWebsites)
					.NotEmpty()
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.PromotingWebsites, ValidationMessages.FieldRequired)
					.Length(0, 1000)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.PromotingWebsites, ValidationMessages.FieldLength,
						1000);

				RuleFor(model => model.InceptionDate)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.InceptionDate, ValidationMessages.FieldRequired);

				RuleFor(model => model.TaxExempt)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.TaxExempt, ValidationMessages.FieldRequired);

				RuleFor(model => model.Tier)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.Tier, ValidationMessages.FieldRequired);

				RuleFor(model => model.TradeClass)
					.Must(model => model.HasValue)
					.When(model => model.CustomerType == CustomerType.Wholesale)
					.WithMessage(model => model.TradeClass, ValidationMessages.FieldRequired);


				RuleFor(model => model.Email)
					.NotEmpty()
                    .When(model=>model.Id!=0 && model.StatusCode!=(int)CustomerStatus.PhoneOnly)
					.WithMessage(model => model.Email, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.Email, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.EmailAddress()
                    .When(model => model.Id != 0 && model.StatusCode != (int)CustomerStatus.PhoneOnly && !string.IsNullOrEmpty(model.Email))
                    .WithMessage(model => model.Email, ValidationMessages.EmailFormat);


				RuleFor(model => model.EmailConfirm)
					.NotEmpty()
                    .When(model => model.Id != 0 && model.StatusCode != (int)CustomerStatus.PhoneOnly)
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.EmailConfirm, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.EmailAddress()
                    .When(model => model.Id != 0 && model.StatusCode != (int)CustomerStatus.PhoneOnly && !string.IsNullOrEmpty(model.EmailConfirm))
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.EmailFormat)
					.Equal(x => x.Email)
                    .When(model => !string.IsNullOrEmpty(model.Email) || !string.IsNullOrEmpty(model.EmailConfirm))
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.EmailMustMatch);

                RuleFor(model => model.DefaultPaymentMethod)
					.Must(model => model.HasValue)
					.WithMessage(model => model.DefaultPaymentMethod, ValidationMessages.FieldRequired);

				RuleFor(model => model.Reason)
					.NotEmpty()
					.When(p => p.StatusCode == (int)CustomerStatus.Suspended)
					.WithMessage(model => model.Reason, ValidationMessages.FieldRequired)
					.Length(0, 1000)
					.When(p => p.StatusCode == (int)CustomerStatus.Suspended)
					.WithMessage(model => model.Reason, ValidationMessages.FieldLength, 1000);

				RuleFor(model => model.ApprovedPaymentMethods)
					.Must(model => model.Count > 0)
					.WithMessage(model => model.ApprovedPaymentMethods, ValidationMessages.AtLeastOnePaymentMethod);
			}
		}
	}
}