using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VC.Public.Models.Affiliate;
using System;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Public.Validators.Affiliate
{
    public class AffiliateManageModelValidator : ModelValidator<AffiliateManageModel>
    {
        public override void Validate(AffiliateManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<AffiliateManageInnerModelValidator>().Validate(value));
        }

        private class AffiliateManageInnerModelValidator : AbstractValidator<AffiliateManageModel>
        {
            public AffiliateManageInnerModelValidator()
            {
                RuleFor(model => model.WebSite)
                 .NotEmpty()
                 .When(p => p.PromoteByWebsite)
                 .WithMessage(model => model.WebSite, ValidationMessages.FieldRequired);

                RuleFor(model => model.Facebook)
                 .NotEmpty()
                 .When(p => p.PromoteByFacebook)
                 .WithMessage(model => model.Facebook, ValidationMessages.FieldRequired);

                RuleFor(model => model.Twitter)
                 .NotEmpty()
                 .When(p => p.PromoteByTwitter)
                 .WithMessage(model => model.Twitter, ValidationMessages.FieldRequired);

                RuleFor(model => model.Blog)
                 .NotEmpty()
                 .When(p => p.PromoteByBlog)
                 .WithMessage(model => model.Blog, ValidationMessages.FieldRequired);

                RuleFor(model => model.MonthlyEmailsSent)
                 .Must(p => p.HasValue)
                 .When(p => p.PromoteByEmails)
                 .WithMessage(model => model.MonthlyEmailsSent, ValidationMessages.FieldRequired);

                RuleFor(model => model.ProfessionalPractice)
                 .Must(p => p.HasValue)
                 .When(p => p.PromoteByProfessionalPractice)
                 .WithMessage(model => model.ProfessionalPractice, ValidationMessages.FieldRequired);

                RuleFor(model => model.Email)
                 .NotEmpty()
                 .When(p =>String.IsNullOrEmpty(p.CurrentEmail))
                 .WithMessage(model => model.Email, ValidationMessages.FieldRequired);

                RuleFor(model => model.ConfirmEmail)
                 .NotEmpty()
                 .When(p => String.IsNullOrEmpty(p.CurrentEmail))
                 .WithMessage(model => model.ConfirmEmail, ValidationMessages.FieldRequired);

                RuleFor(model => model.Password)
                 .NotEmpty()
                 .When(p => String.IsNullOrEmpty(p.CurrentEmail))
                 .WithMessage(model => model.Password, ValidationMessages.FieldRequired);

                RuleFor(model => model.ConfirmPassword)
                 .NotEmpty()
                 .When(p => String.IsNullOrEmpty(p.CurrentEmail))
                 .WithMessage(model => model.ConfirmPassword, ValidationMessages.FieldRequired);
            }
        }
    }
}