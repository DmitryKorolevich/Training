using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;

namespace VC.Admin.Validators.Product
{
    public class DiscountManageModelValidator : ModelValidator<DiscountManageModel>
    {
        public override void Validate(DiscountManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<DiscountModelValidator>().Validate(value));
        }

        private class DiscountModelValidator : AbstractValidator<DiscountManageModel>
        {
            public DiscountModelValidator()
            {
                RuleFor(model => model.Code)
                    .NotEmpty()
                    .WithMessage(model => model.Code, ValidationMessages.FieldRequired);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);

                RuleFor(model => model.RequireMinimumPerishableAmount)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p =>p.DiscountType!=DiscountType.FreeShipping && p.RequireMinimumPerishable)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.DiscountType != DiscountType.FreeShipping && p.RequireMinimumPerishable)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 100000)
                    .WithName("Require Minimum Perishable Amount");
            }
        }
    }
}