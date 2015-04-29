﻿using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Models.Account;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Validators.Account
{
    public class CreateAccountValidator : ModelValidator<CreateAccountModel>
	{
		public override void Validate(CreateAccountModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<AccountRuleSet>().Validate(value));
		}

		private class AccountRuleSet : AbstractValidator<CreateAccountModel>
		{
			public AccountRuleSet()
			{
				RuleFor(model => model.FirstName)
					.NotEmpty()
					.WithMessage(model => model.FirstName, ValidationMessages.FieldRequired);
				RuleFor(model => model.FirstName)
					.Length(0,100)
					.WithMessage(model => model.FirstName, ValidationMessages.FieldLength);

				RuleFor(model => model.LastName)
					.NotEmpty()
					.WithMessage(model => model.LastName, ValidationMessages.FieldRequired);
				RuleFor(model => model.LastName)
					.Length(0, 100)
					.WithMessage(model => model.LastName, ValidationMessages.FieldLength);

				RuleFor(model => model.Email).NotEmpty().WithMessage(model => model.Email, ValidationMessages.FieldRequired);
				RuleFor(model => model.Email).Length(3, 100).WithMessage(model => model.Email, ValidationMessages.FieldLength);
				RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, "Incorrect email format");

				RuleFor(model => model.Password).NotEmpty()
					.WithMessage(model => model.Password, ValidationMessages.FieldRequired);

				RuleFor(model => model.ConfirmPassword).NotEmpty()
					.WithMessage(model => model.ConfirmPassword, ValidationMessages.FieldRequired);

				RuleFor(model => model.Password)
					.Equal(x=>x.ConfirmPassword)
					.WithMessage(model => model.LastName, "Password should match password confirmation");
			}
		}
	}
}