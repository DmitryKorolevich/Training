using VitalChoice.Validation.Logic;
using FluentValidation;
using VC.Admin.Models.Account;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.Account
{
    public class ResetPasswordValidator : ModelValidator<ResetPasswordModel>
	{
		public override void Validate(ResetPasswordModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<ResetPasswordRuleSet>().Validate(value));
		}

		private class ResetPasswordRuleSet : AbstractValidator<ResetPasswordModel>
		{
			public ResetPasswordRuleSet()
			{
				RuleFor(model => model.Email).NotEmpty().WithMessage(model => model.Email, ValidationMessages.FieldRequired);
				RuleFor(model => model.Email).Length(0, 100).WithMessage(model => model.Email, ValidationMessages.FieldLength, 100);
				RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, ValidationMessages.EmailFormat);

				RuleFor(model => model.Password).NotEmpty()
					.WithMessage(model => model.Password, ValidationMessages.FieldRequired);

				RuleFor(model => model.ConfirmPassword).NotEmpty()
					.WithMessage(model => model.ConfirmPassword, ValidationMessages.FieldRequired);

				RuleFor(model => model.Password)
					.Equal(x=>x.ConfirmPassword)
					.WithMessage(model => model.Password, ValidationMessages.PasswordMustMatch);
			}
		}
	}
}