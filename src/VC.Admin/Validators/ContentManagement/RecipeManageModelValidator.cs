using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
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
			foreach (var crossRecipe in value.CrossSellRecipes)
			{
				ParseResults(crossSellRecipeValidator.Validate(crossRecipe));
			}

			var relatedRecipeValidator = ValidatorsFactory.GetValidator<RelatedRecipeModelValidator.RelatedRecipeRules>();
			foreach (var relatedRecipe in value.RelatedRecipes)
			{
				ParseResults(relatedRecipeValidator.Validate(relatedRecipe));
			}

			var videoRecipeValidator = ValidatorsFactory.GetValidator<RecipeVideoModelValidator.VideoRecipeRules>();
			foreach (var videoRecipe in value.Videos)
			{
				ParseResults(videoRecipeValidator.Validate(videoRecipe));
			}
		}

        private class RecipeModelValidator : AbstractValidator<RecipeManageModel>
        {
            public RecipeModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);
            }
        }
    }
}