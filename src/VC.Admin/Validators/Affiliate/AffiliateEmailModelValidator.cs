using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Admin.Models.Affiliate;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Affiliate
{
    public class AffiliateEmailManageModelValidator : ModelValidator<AffiliateEmailModel>
    {
        public override void Validate(AffiliateEmailModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<AffiliateEmailModelValidator>().Validate(value));
        }

        private class AffiliateEmailModelValidator : AbstractValidator<AffiliateEmailModel>
        {
            public AffiliateEmailModelValidator()
            {
                RuleFor(model => model.ToEmail)
                    .Matches(ValidationPatterns.EmailPattern)
                    .WithMessage(model => model.ToEmail, ValidationMessages.EmailFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ToEmail, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.FromEmail)
                    .Matches(ValidationPatterns.EmailPattern)
                    .WithMessage(model => model.ToEmail, ValidationMessages.EmailFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ToEmail, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Subject)
                    .NotEmpty()
                    .WithMessage(model => model.Subject, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Subject, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}