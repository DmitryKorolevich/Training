using System.Linq;
using FluentValidation;
using VC.Admin.Models.UserManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.UserManagement
{
    public class UserManageAdminValidator: ModelValidator<ManageUserModel>
    {
		public override void Validate(ManageUserModel value)
		{
			ValidationErrors.Clear();

			var validator = ValidatorsFactory.GetValidator<UserRuleSet>();
            ParseResults(validator.Validate(value, ruleSet: "Main"));
			if(value.Mode.Mode == UserManageMode.Update)
			{
				ParseResults(validator.Validate(value, ruleSet: "Update"));
			}
			
		}

		private class UserRuleSet :  AbstractValidator<ManageUserModel>
		{
		    public UserRuleSet()
		    {
			    RuleSet
				    ("Main",
					    () =>
					    {
						    RuleFor(model => model.AgentId)
							    .NotEmpty()
							    .WithMessage(model => model.AgentId, ValidationMessages.FieldRequired);
						    RuleFor(model => model.AgentId)
							    .Length(0, 10)
							    .WithMessage(model => model.AgentId, ValidationMessages.FieldLength, 10);

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
						    RuleFor(model => model.Email).Length(0, 100).WithMessage(model => model.Email, ValidationMessages.FieldLength, 100);
						    RuleFor(model => model.Email).EmailAddress().WithMessage(model => model.Email, ValidationMessages.EmailFormat);

						    RuleFor(model => model.RoleIds)
							    .Must(x => x.Any())
							    .WithMessage(model => model.RoleIds, ValidationMessages.AtLeastOneRole);
					    });

				RuleSet
					("Update",
						() =>
						{
							//RuleFor(model => model.Status)
							//	.Must(x => x == UserStatus.Active || x == UserStatus.Disabled)
							//	.WithMessage(model => model.Status, ValidationMessages.UserStatusRestriction);
						});
			}
		}
    }
}