using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using System;
using VC.Admin.Models.Products;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Product
{
    public class ProductReviewManageModelValidator : ModelValidator<ProductReviewManageModel>
    {
        public override void Validate(ProductReviewManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ProductReviewModelValidator>().Validate(value));
        }

        private class ProductReviewModelValidator : AbstractValidator<ProductReviewManageModel>
        {
            public ProductReviewModelValidator()
            {
                RuleFor(model => model.CustomerName)
                    .NotEmpty()
                    .WithMessage(model => model.CustomerName, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.CustomerName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Email)
                    .Matches(ValidationPatterns.EmailPattern)
                    .When(p => !String.IsNullOrEmpty(p.Email))
                    .WithMessage(model => model.Email, ValidationMessages.EmailFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Email, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Title)
                    .NotEmpty()
                    .WithMessage(model => model.Title, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Description, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}