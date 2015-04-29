using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Models.Account;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Validators.Account
{
    public class LoginValidator : ModelValidator<LoginModel>
	{
		public override void Validate(LoginModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<LoginRuleSet>().Validate(value));
		}

		private class LoginRuleSet : AbstractValidator<LoginModel>
		{
			public LoginRuleSet()
			{
				RuleFor(model => model.Email).NotEmpty().WithMessage(model => model.Email, ValidationMessages.FieldRequired);
				RuleFor(model => model.Email).Length(3, 100).WithMessage(model => model.Email, ValidationMessages.FieldLength);
				RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, "Incorrect email format");

				RuleFor(model => model.Password).NotEmpty()
					.WithMessage(model => model.Password, ValidationMessages.FieldRequired);

			}
		}
	}
}