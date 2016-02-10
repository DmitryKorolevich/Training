using FluentValidation;
using VC.Admin.Models.Products;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Validators.Product
{
    public class PromotionManageModelValidator : ModelValidator<PromotionManageModel>
    {
        public override void Validate(PromotionManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<PromotionModelValidator>().Validate(value));
            if(value.IdObjectType== PromotionType.BuyXGetY)
            {
                if (value.PromotionsToBuySkus != null)
                {
                    for (int i = 0; i < value.PromotionsToBuySkus.Count; i++)
                    {
                        ParseResults(ValidatorsFactory.GetValidator<PromotionToBuySkuModelValidator>().Validate(value.PromotionsToBuySkus[i]), "PromotionsToBuySkus", i);
                    }
                }
                if (value.PromotionsToGetSkus != null)
                {
                    for (int i = 0; i < value.PromotionsToGetSkus.Count; i++)
                    {
                        ParseResults(ValidatorsFactory.GetValidator<PromotionToGetSkuModelValidator>().Validate(value.PromotionsToGetSkus[i]), "PromotionsToGetSkus", i);
                    }
                }
            }
        }

        private class PromotionModelValidator : AbstractValidator<PromotionManageModel>
        {
            public PromotionModelValidator()
            {
                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Description, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.StartDate)
                    .Must(model=> model.HasValue)
                    .WithMessage(model => model.StartDate, ValidationMessages.FieldRequired);

                RuleFor(model => model.ExpirationDate)
                    .Must(model => model.HasValue)
                    .WithMessage(model => model.StartDate, ValidationMessages.FieldRequired);

                RuleFor(model => model.Percent)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .GreaterThan(0)
                    .When(p => p.IdObjectType == PromotionType.CategoryDiscount)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100)
                    .When(p => p.IdObjectType == PromotionType.CategoryDiscount)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Percent, 100);
            }
        }

        private class PromotionToBuySkuModelValidator : AbstractValidator<PromotionToBuySkuModel>
        {
            public PromotionToBuySkuModelValidator()
            {
                RuleFor(model => model.Quantity)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(p=>p.HasValue)
                    .WithMessage(model => model.Quantity, ValidationMessages.FieldRequired)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Quantity, 0)
                    .LessThan(100)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Quantity, 100);
            }
        }

        private class PromotionToGetSkuModelValidator : AbstractValidator<PromotionToGetSkuModel>
        {
            public PromotionToGetSkuModelValidator()
            {
                RuleFor(model => model.Quantity)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(p => p.HasValue)
                    .WithMessage(model => model.Quantity, ValidationMessages.FieldRequired)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Quantity, 0)
                    .LessThan(100)
                    .WithMessage(ValidationMessages.FieldMax, GeneralFieldNames.Quantity, 100);

                RuleFor(model => model.Percent)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(p => p.HasValue)
                    .WithMessage(model => model.Percent, ValidationMessages.FieldRequired)
                    .GreaterThan(0)
                    .WithMessage(ValidationMessages.FieldMin, GeneralFieldNames.Percent, 0)
                    .LessThanOrEqualTo(100000)
                    .WithMessage(ValidationMessages.FieldMaxOrEqual, GeneralFieldNames.Percent, 100);
            }
        }
    }
}