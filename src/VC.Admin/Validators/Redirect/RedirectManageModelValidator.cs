using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VC.Admin.Models.Redirects;

namespace VC.Admin.Validators.Redirect
{
    public class RedirectManageModelValidator : ModelValidator<RedirectManageModel>
    {
        public override void Validate(RedirectManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<RedirectManageValidator>().Validate(value));
        }

        private class RedirectManageValidator : AbstractValidator<RedirectManageModel>
        {
            public RedirectManageValidator()
            {
                RuleFor(model => model.From)
                    .NotEmpty()
                    .WithMessage(model => model.From, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.From, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .Matches(ValidationPatterns.RelativeUrlPattern)
                    .WithMessage(model => model.From, ValidationMessages.FieldRelativeUrlInvalidFormat);

                RuleFor(model => model.To)
                    .NotEmpty()
                    .WithMessage(model => model.To, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.To, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}