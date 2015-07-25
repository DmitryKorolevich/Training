﻿using System.Linq;
using FluentValidation;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.Settings;

namespace VC.Admin.Validators.Customer
{
	public class AddressModelValidator : ModelValidator<AddressModel>
	{
		public override void Validate(AddressModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<AddressModelRules>().Validate(value));
		}
	}

	public class AddressModelRules : AbstractValidator<AddressModel>
	{
		public AddressModelRules()
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

			RuleFor(model => model.Address1)
				.NotEmpty()
				.WithMessage(model => model.Address1, ValidationMessages.FieldRequired)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Address1, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.Address2)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Address2, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.City)
				.NotEmpty()
				.WithMessage(model => model.City, ValidationMessages.FieldRequired)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.City, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.Country)
				.Must(model => model.Id != 0)
				.WithMessage(model => model.Country, ValidationMessages.FieldRequired);

			RuleFor(model => model.State)
				.Must(model => model != 0)
				.When(model => model.Country.States.Any())
				.WithMessage(model => model.State, ValidationMessages.FieldRequired);

			RuleFor(model => model.County)
				.NotEmpty()
				.When(model => !model.Country.States.Any())
				.WithMessage(model => model.County, ValidationMessages.FieldRequired);

			RuleFor(model => model.Zip)
				.NotEmpty()
				.WithMessage(model => model.Zip, ValidationMessages.FieldRequired)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Zip, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.Phone)
				.NotEmpty()
				.WithMessage(model => model.Phone, ValidationMessages.FieldRequired)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Phone, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.Fax)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Fax, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

			RuleFor(model => model.Email)
				.NotEmpty()
				.When(x => x.AddressType == AddressType.Billing)
				.WithMessage(model => model.Email, ValidationMessages.FieldRequired)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Email, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(model => model.Email, ValidationMessages.EmailFormat);
		}
	}
}