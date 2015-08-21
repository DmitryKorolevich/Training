using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Domain.Constants;
using VC.Admin.Models.Affiliate;

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

                RuleFor(model => model.Message)
                    .NotEmpty()
                    .WithMessage(model => model.Message, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_EMAIL_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Message, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_EMAIL_FIELD_MAX_SIZE);
            }
        }
    }
}