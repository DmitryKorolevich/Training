using FluentValidation;
using VC.Admin.Models.Account;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.Account
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
				RuleFor(model => model.Email).Length(0,100).WithMessage(model => model.Email, ValidationMessages.FieldLength, 100);
				RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, ValidationMessages.EmailFormat);

				RuleFor(model => model.Password).NotEmpty()
					.WithMessage(model => model.Password, ValidationMessages.FieldRequired);

			}
		}
	}
}