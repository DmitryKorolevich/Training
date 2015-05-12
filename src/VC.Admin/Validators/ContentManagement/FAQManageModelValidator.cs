using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class FaqManageModelValidator : ModelValidator<FAQManageModel>
    {
        public override void Validate(FAQManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<FAQModelValidator>().Validate(value));
        }

        private class FAQModelValidator : AbstractValidator<FAQManageModel>
        {
            public FAQModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);
            }
        }
    }
}