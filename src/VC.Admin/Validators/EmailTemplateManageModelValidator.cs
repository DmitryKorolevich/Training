using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VC.Admin.Models.EmailTemplates;

namespace VC.Admin.Validators
{
    public class EmailTemplateManageModelValidator : ModelValidator<EmailTemplateManageModel>
    {
        public override void Validate(EmailTemplateManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<EmailTemplateModelValidator>().Validate(value));
		}

        private class EmailTemplateModelValidator : AbstractValidator<EmailTemplateManageModel>
        {
            public EmailTemplateModelValidator()
            {
                RuleFor(model => model.Subject)
                    .NotEmpty()
                    .WithMessage(model => model.Subject, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				    .WithMessage(model => model.Subject, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Template)
                    .NotEmpty()
                    .WithMessage(model => model.Template, ValidationMessages.FieldRequired);
            }
        }
    }
}