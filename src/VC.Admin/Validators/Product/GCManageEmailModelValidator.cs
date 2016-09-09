using FluentValidation;
using VC.Admin.Models.Products;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

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
                    .WithMessage(model => model.ToEmail, ValidationMessages.EmailFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ToEmail, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Message)
                    .Length(0, BaseAppConstants.DEFAULT_EMAIL_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Message, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_EMAIL_FIELD_MAX_SIZE);
            }
        }
    }
}