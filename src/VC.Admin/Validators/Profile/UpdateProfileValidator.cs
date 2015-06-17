using VitalChoice.Validation.Logic;
using FluentValidation;
using VC.Admin.Models.Profile;
using VC.Admin.Validators.Profile;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.Profile
{
    public class UpdateProfileValidator : ModelValidator<UpdateProfileModel>
	{
		public override void Validate(UpdateProfileModel value)
		{
			var validator = ValidatorsFactory.GetValidator<ProfileRuleSet>();
			ParseResults(validator.Validate(value, ruleSet: "Main"));
			if(value.Mode.Mode == UpdateProfileMode.WithPassword)
			{
				ParseResults(validator.Validate(value, ruleSet: "WithPassword"));
			}
		}

		private class ProfileRuleSet : AbstractValidator<UpdateProfileModel>
		{
			public ProfileRuleSet()
			{
				RuleSet
					("Main",
						() =>
						{
							RuleFor(model => model.FirstName)
								.NotEmpty()
								.WithMessage(model => model.FirstName, ValidationMessages.FieldRequired);
							RuleFor(model => model.FirstName)
								.Length(0, 100)
								.WithMessage(model => model.FirstName, ValidationMessages.FieldLength, 100);

							RuleFor(model => model.LastName)
								.NotEmpty()
								.WithMessage(model => model.LastName, ValidationMessages.FieldRequired);
							RuleFor(model => model.LastName)
								.Length(0, 100)
								.WithMessage(model => model.LastName, ValidationMessages.FieldLength, 100);

							RuleFor(model => model.Email).NotEmpty().WithMessage(model => model.Email, ValidationMessages.FieldRequired);
							RuleFor(model => model.Email)
								.Length(0, 100)
								.WithMessage(model => model.Email, ValidationMessages.FieldLength, 100);
							RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, ValidationMessages.EmailFormat);
						});
				RuleSet
					("WithPassword",
						() =>
						{
							RuleFor(model => model.OldPassword).NotEmpty()
								.WithMessage(model => model.OldPassword, ValidationMessages.FieldRequired);

							RuleFor(model => model.NewPassword).NotEmpty()
								.WithMessage(model => model.NewPassword, ValidationMessages.FieldRequired);

							RuleFor(model => model.ConfirmNewPassword).NotEmpty()
								.WithMessage(model => model.ConfirmNewPassword, ValidationMessages.FieldRequired);

							RuleFor(model => model.NewPassword)
								.Equal(x => x.ConfirmNewPassword)
								.WithMessage(model => model.NewPassword, ValidationMessages.PasswordMustMatch);
						});
			}
		}
	}
}