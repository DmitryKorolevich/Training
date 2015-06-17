using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;
using VitalChoice.Infrastructure.Utils;

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
                    .WithMessage(model => model.Email, ValidationMessages.EmailFormat);
            }
        }
    }
}