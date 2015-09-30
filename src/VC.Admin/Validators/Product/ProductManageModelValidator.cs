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
    public class CrossSellProductModelValidator : AbstractValidator<CrossSellProductModel>
    {
        public CrossSellProductModelValidator()
        {
            RuleFor(model => model.Image)
                .NotEmpty()
                .WithMessage(model => model.Image, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

            RuleFor(model => model.Url)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                .WithMessage(model => model.Url, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
        }
    }

    public class VideoModelValidator : AbstractValidator<VideoModel>
    {
        public VideoModelValidator()
        {
            RuleFor(model => model.Image)
                .NotEmpty()
                .WithMessage(model => model.Image, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

            RuleFor(model => model.Video)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(model => model.Video, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                .WithMessage(model => model.Video, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

            RuleFor(model => model.Text)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(model => model.Text, ValidationMessages.FieldRequired)
                .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                .WithMessage(model => model.Text, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
        }
    }

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

            var crossSellProductModelValidator = ValidatorsFactory.GetValidator<CrossSellProductModelValidator>();
            for (int i = 0; i < value.CrossSellProducts.Count; i++)
            {
                if (!value.CrossSellProducts[i].IsDefault)
                {
                    ParseResults(crossSellProductModelValidator.Validate(value.CrossSellProducts[i]), "CrossSellProducts", i);
                }
            }

            var videoModelValidator = ValidatorsFactory.GetValidator<VideoModelValidator>();
            for (int i = 0; i < value.Videos.Count; i++)
            {
                if (!value.Videos[i].IsDefault)
                {
                    ParseResults(videoModelValidator.Validate(value.Videos[i]), "Videos", i);
                }
            }
        }

        private class ProductModelValidator : AbstractValidator<ProductManageModel>
        {
            public ProductModelValidator()
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

                RuleFor(model => model.MetaTitle)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.MetaTitle, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.MetaDescription)
                    .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                    .WithMessage(model => model.MetaDescription, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                RuleFor(model => model.GoogleFeedTitle)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.GoogleFeedTitle, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.GoogleFeedDescription)
                    .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                    .WithMessage(model => model.GoogleFeedDescription, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                RuleFor(model => model.TaxCode)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TaxCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.DescriptionTitleOverride)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.DescriptionTitleOverride, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.ServingTitleOverride)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ServingTitleOverride, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.RecipesTitleOverride)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.RecipesTitleOverride, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.IngredientsTitleOverride)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.IngredientsTitleOverride, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.ShippingTitleOverride)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ShippingTitleOverride, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.SubProductGroupName)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SubProductGroupName, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                RuleFor(model => model.NutritionalTitle)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.NutritionalTitle, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.ServingSize)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ServingSize, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Servings)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Servings, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Calories)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Calories, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.CaloriesFromFat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.CaloriesFromFat, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TotalFat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TotalFat, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TotalFatPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TotalFatPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.SaturatedFat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SaturatedFat, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.SaturatedFatPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SaturatedFatPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TransFat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TransFat, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TransFatPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TransFatPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Cholesterol)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Cholesterol, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.CholesterolPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.CholesterolPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Sodium)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Sodium, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.SodiumPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SodiumPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TotalCarbohydrate)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TotalCarbohydrate, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.TotalCarbohydratePercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.TotalCarbohydratePercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.DietaryFiber)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.DietaryFiber, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.DietaryFiberPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.DietaryFiberPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Sugars)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Sugars, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.SugarsPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.SugarsPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.Protein)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Protein, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.ProteinPercent)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.ProteinPercent, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }

        private class SKUModelValidator : AbstractValidator<SKUManageModel>
        {
            public SKUModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                RuleFor(model => model.QTY)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                    .WithMessage(model => model.QTY, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
            }
        }
    }
}