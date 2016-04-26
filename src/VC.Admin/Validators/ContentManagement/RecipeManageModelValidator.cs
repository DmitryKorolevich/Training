using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class RecipeManageModelValidator : ModelValidator<RecipeManageModel>
    {
        public override void Validate(RecipeManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<RecipeModelValidator>().Validate(value));

			var crossSellRecipeValidator = ValidatorsFactory.GetValidator<CrossSellRecipeModelValidator.CrossSellRecipeRules>();
			for (int i = 0; i < value.CrossSellRecipes.Count; i++)
			{
				ParseResults(crossSellRecipeValidator.Validate(value.CrossSellRecipes[i]), "CrossSellRecipes", i);
			}

			var relatedRecipeValidator = ValidatorsFactory.GetValidator<RelatedRecipeModelValidator.RelatedRecipeRules>();
			for (int i = 0; i < value.RelatedRecipes.Count; i++)
			{
				ParseResults(relatedRecipeValidator.Validate(value.RelatedRecipes[i]), "RelatedRecipes", i);
			}
		}

        private class RecipeModelValidator : AbstractValidator<RecipeManageModel>
        {
            public RecipeModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.Name, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

	            RuleFor(model => model.Subtitle)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
					.WithMessage(model => model.Subtitle, ValidationMessages.FieldLength,
			            BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

				RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);

				RuleFor(model => model.YoutubeVideo)
				   .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.YoutubeVideo, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
			}
        }
    }
}