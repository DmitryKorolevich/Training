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
                RuleFor(model => model.MonthlyEmailsSent)
                 .Must(p => p.HasValue)
                 .When(p => p.PromoteByEmails)
                 .WithMessage(model => model.MonthlyEmailsSent, ValidationMessages.FieldRequired);

                RuleFor(model => model.ProfessionalPractice)
                 .Must(p => p.HasValue)
                 .When(p => p.PromoteByProfessionalPractice)
                 .WithMessage(model => model.ProfessionalPractice, ValidationMessages.FieldRequired);
            }
        }
    }
}