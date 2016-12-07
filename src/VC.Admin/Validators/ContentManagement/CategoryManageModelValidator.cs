using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class CategoryManageModelValidator : ModelValidator<CategoryManageModel>
    {
        public override void Validate(CategoryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<CategoryModelValidator>().Validate(value));
        }

        private class CategoryModelValidator : AbstractValidator<CategoryManageModel>
        {
            public CategoryModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);

                RuleFor(model => model.Url)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Url, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);
                RuleFor(model => model.Url)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.MetaDescription)
                    .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                    .WithMessage(model => model.MetaDescription, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                RuleFor(model => model.Title)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);
            }
        }
    }
}