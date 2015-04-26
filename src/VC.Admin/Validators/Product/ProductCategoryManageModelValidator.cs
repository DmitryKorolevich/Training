using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;
using VitalChoice.Models.Product;

namespace VitalChoice.Validators.Product
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
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired);
                RuleFor(model => model.Url)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);
            }
        }
    }
}