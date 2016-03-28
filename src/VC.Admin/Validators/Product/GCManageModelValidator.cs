using FluentValidation;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;
using VC.Admin.Models.Products;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Product
{
    public class GCManageModelValidator : ModelValidator<GCManageModel>
    {
        public override void Validate(GCManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<GCModelValidator>().Validate(value));
        }

        private class GCModelValidator : AbstractValidator<GCManageModel>
        {
            public GCModelValidator()
            {
                RuleFor(model => model.Balance)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 100000)
                    .WithName("Balance");

                RuleFor(model => model.Email)
                    .Matches(ValidationPatterns.EmailPattern)
                    .When(p=>!String.IsNullOrEmpty(p.Email))
                    .WithMessage(model => model.Email, ValidationMessages.EmailFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Email, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.FirstName)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.FirstName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.LastName)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.LastName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}