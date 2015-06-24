using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;

namespace VC.Admin.Validators.Product
{
    public class DiscountManageModelValidator : ModelValidator<DiscountManageModel>
    {
        public override void Validate(DiscountManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<DiscountModelValidator>().Validate(value));
            if (value.DiscountTiers != null)
            {
                for (int i = 0; i < value.DiscountTiers.Count; i++)
                {
                    ParseResults(ValidatorsFactory.GetValidator<DiscountTierValidator>().Validate(value.DiscountTiers[i]), "DiscountTiers", i);
                }
            }
        }

        private class DiscountModelValidator : AbstractValidator<DiscountManageModel>
        {
            public DiscountModelValidator()
            {
                RuleFor(model => model.Code)
                    .NotEmpty()
                    .WithMessage(model => model.Code, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Code, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Description, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.RequireMinimumPerishableAmount)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.DiscountType != DiscountType.FreeShipping && p.RequireMinimumPerishable)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.DiscountType != DiscountType.FreeShipping && p.RequireMinimumPerishable)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Amount, 100000);


                RuleFor(model => model.Amount)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.DiscountType == DiscountType.PriceDiscount)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.DiscountType == DiscountType.PriceDiscount)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Amount, 100000);

                RuleFor(model => model.Percent)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.DiscountType == DiscountType.PercentDiscount)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.DiscountType == DiscountType.PercentDiscount)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Percent, 100);

                RuleFor(model => model.Threshold)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.DiscountType == DiscountType.Threshold)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.DiscountType == DiscountType.Threshold)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Amount, 100000);

                RuleFor(model => model.ProductSKU)
                    .NotEmpty()
                    .When(p => p.DiscountType == DiscountType.Threshold)
                    .WithMessage(ValidationMessages.FieldRequired, GeneralFieldNames.Code)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .When(p => p.DiscountType == DiscountType.Threshold)
                    .WithMessage(model => model.ProductSKU, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }

        private class DiscountTierValidator : AbstractValidator<DiscountTier>
        {
            public DiscountTierValidator()
            {
                RuleFor(model => model.From)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage(ValidationMessages.FieldMinOrEqual, GeneralFieldNames.Min, 0)
                    .LessThanOrEqualTo(100000)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Min, 100000);

                RuleFor(model => model.To)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Max, 0)
                    .LessThanOrEqualTo(100000)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Max, 100000);

                RuleFor(model => model.Amount)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.IdDiscountType == DiscountType.PriceDiscount)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Amount, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.IdDiscountType == DiscountType.PriceDiscount)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Amount, 100000);

                RuleFor(model => model.Amount)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.IdDiscountType == DiscountType.PercentDiscount)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100000)
                    .When(p => p.IdDiscountType == DiscountType.PercentDiscount)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Percent, 100);
            }
        }
    }
}