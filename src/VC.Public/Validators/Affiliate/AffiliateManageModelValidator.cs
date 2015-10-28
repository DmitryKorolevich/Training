using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;
using VC.Public.Models.Auth;
using VC.Public.Models;
using VC.Public.Models.Affiliate;

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

                RuleFor(model => model.IsAllowAgreement)
                 .Must(p=>p)
                 .WithMessage("Please agree to Web Affiliate Agreement");

                RuleFor(model => model.IsNotSpam)
                 .Must(p => p)
                 .WithMessage("Please agree to SPAM Agreement");
            }
        }
    }
}