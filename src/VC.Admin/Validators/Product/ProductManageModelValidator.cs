using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.Product
{
    public class ProductManageModelValidator : ModelValidator<ProductManageModel>
    {
        public override void Validate(ProductManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ProductModelValidator>().Validate(value));
            if (value.SKUs != null)
            {
                for (int i = 0; i < value.SKUs.Count; i++)
                {
                    ParseResults(ValidatorsFactory.GetValidator<SKUModelValidator>().Validate(value.SKUs[i]), "SKUs", i);
                }
            }
        }

        private class ProductModelValidator : AbstractValidator<ProductManageModel>
        {
            public ProductModelValidator()
            {
                //RuleFor(model => model.Name)
                //    .NotEmpty()
                //    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);
            }
        }

        private class SKUModelValidator : AbstractValidator<SKUManageModel>
        {
            public SKUModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);
            }
        }
    }
}