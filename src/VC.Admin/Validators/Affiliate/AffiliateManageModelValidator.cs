using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using System;
using VC.Admin.Models.Affiliates;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Affiliate
{
    public class AffiliatetManageModelValidator : ModelValidator<AffiliateManageModel>
    {
        public override void Validate(AffiliateManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<AffiliateModelValidator>().Validate(value));
        }

        private class AffiliateModelValidator : AbstractValidator<AffiliateManageModel>
        {
            public AffiliateModelValidator()
            {
                RuleFor(model => model.MyAppBalance)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Balance, 0)
                    .LessThanOrEqualTo(100000)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Balance, 100000);

                RuleFor(model => model.MonthlyEmailsSent)
                    .Must(model => model.HasValue)
                    .When(model => model.PromoteByEmails)
                    .WithMessage(model => model.MonthlyEmailsSent, ValidationMessages.FieldRequired);

                RuleFor(model => model.Facebook)
                    .NotEmpty()
                    .When(model => model.PromoteByFacebook)
                    .WithMessage(model => model.Facebook, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .When(p => p.PromoteByFacebook)
                    .WithMessage(model => model.Facebook, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                
                RuleFor(model => model.Twitter)
                    .NotEmpty()
                    .When(model => model.PromoteByTwitter)
                    .WithMessage(model => model.Twitter, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .When(p => p.PromoteByTwitter)
                    .WithMessage(model => model.Twitter, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Blog)
                    .NotEmpty()
                    .When(model => model.PromoteByBlog)
                    .WithMessage(model => model.Blog, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .When(p => p.PromoteByBlog)
                    .WithMessage(model => model.Blog, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.ProfessionalPractice)
                    .Must(model => model.HasValue)
                    .When(model => model.PromoteByProfessionalPractice)
                    .WithMessage(model => model.ProfessionalPractice, ValidationMessages.FieldRequired);

                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.ChecksPayableTo)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ChecksPayableTo, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.TaxID)
                    .NotEmpty()
                    .WithMessage(model => model.Address1, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TaxID, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Address1)
                    .NotEmpty()
                    .WithMessage(model => model.Address1, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Address2, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Address2)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Address2, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Company)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Company, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.IdCountry)
                    .Must(model => model.HasValue)
                    .WithMessage(model => model.IdCountry, ValidationMessages.FieldRequired);

                RuleFor(model => model.IdState)
                    .Must(model => model.HasValue)
                    .When(model =>String.IsNullOrEmpty(model.County))
                    .WithMessage(model => model.IdState, ValidationMessages.FieldRequired);

                RuleFor(model => model.County)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .When(model => !model.IdState.HasValue)
                    .WithMessage(model => model.County, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                
                RuleFor(model => model.City)
                    .NotEmpty()
                    .WithMessage(model => model.City, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.City, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                
                RuleFor(model => model.Zip)
                    .NotEmpty()
                    .WithMessage(model => model.Zip, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Zip, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.HearAbout)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.HearAbout, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.WebSite)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.WebSite, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Reach)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Reach, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Profession)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Profession, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Tier)
                    .Must(model => model != 0)
                    .WithMessage(model => model.Tier, ValidationMessages.FieldRequired);
                
                RuleFor(model => model.PaymentType)
                    .Must(model => model != 0)
                    .WithMessage(model => model.PaymentType, ValidationMessages.FieldRequired);

                RuleFor(model => model.CommissionFirst)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Percent, 100);

                RuleFor(model => model.CommissionAll)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Percent, 100);

                RuleFor(model => model.Email)
                    .NotEmpty()
                    .WithMessage(model => model.Email, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Email, ValidationMessages.FieldLength,
                        BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .EmailAddress()
                    .WithMessage(model => model.Email, ValidationMessages.EmailFormat);


                RuleFor(model => model.EmailConfirm)
                    .NotEmpty()
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.FieldLength,
                        BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .EmailAddress()
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.EmailFormat)
                    .Equal(x => x.Email)
                    .WithMessage(model => model.EmailConfirm, ValidationMessages.EmailMustMatch);
            }
        }
    }
}