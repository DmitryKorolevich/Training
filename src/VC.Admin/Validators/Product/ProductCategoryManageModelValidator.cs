using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Domain.Constants;

namespace VC.Admin.Validators.Product
{
    public class ProductCategoryManageModelValidator : ModelValidator<ProductCategoryManageModel>
    {
        public override void Validate(ProductCategoryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ProductCategoryModelValidator>().Validate(value));
        }

        private class ProductCategoryModelValidator : AbstractValidator<ProductCategoryManageModel>
        {
            public ProductCategoryModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Url)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Url, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Url)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Title)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.MetaDescription)
                    .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                    .WithMessage(model => model.MetaDescription, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                RuleFor(model => model.Description)
                    .Length(0, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.MetaDescription, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.NavLabel)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.NavLabel, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}