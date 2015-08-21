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
			for (int i = 0; i < value.CrossSellRecipes.Count; i++)
			{
				ParseResults(crossSellRecipeValidator.Validate(value.CrossSellRecipes[i]), "CrossSellRecipes", i);
			}

			var relatedRecipeValidator = ValidatorsFactory.GetValidator<RelatedRecipeModelValidator.RelatedRecipeRules>();
			for (int i = 0; i < value.RelatedRecipes.Count; i++)
			{
				ParseResults(relatedRecipeValidator.Validate(value.RelatedRecipes[i]), "RelatedRecipes", i);
			}

			var videoRecipeValidator = ValidatorsFactory.GetValidator<RecipeVideoModelValidator.VideoRecipeRules>();
			for (int i = 0; i < value.Videos.Count; i++)
			{
				ParseResults(videoRecipeValidator.Validate(value.Videos[i]), "CrossRelatedMiscellaneous", i);
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