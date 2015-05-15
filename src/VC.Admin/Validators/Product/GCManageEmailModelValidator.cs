using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;

namespace VC.Admin.Validators.Product
{
    public class GCManageEmailModelValidator : ModelValidator<GCEmailModel>
    {
        public override void Validate(GCEmailModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<GCEmailModelValidator>().Validate(value));
        }

        private class GCEmailModelValidator : AbstractValidator<GCEmailModel>
        {
            public GCEmailModelValidator()
            {
                RuleFor(model => model.ToEmail)
                    .Matches(ValidationPatterns.EmailPattern)
                    .WithMessage(model => model.ToEmail, ValidationMessages.EmailFormat);
                
                RuleFor(model => model.Message)
                    .NotEmpty()
                    .WithMessage(model => model.Message, ValidationMessages.FieldRequired);
            }
        }
    }
}