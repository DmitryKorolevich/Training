using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.ContentManagement
{
	public class RelatedRecipeModelValidator : ModelValidator<RelatedRecipeModel>
	{
		public override void Validate(RelatedRecipeModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<RelatedRecipeRules>().Validate(value));
		}

		public class RelatedRecipeRules : AbstractValidator<RelatedRecipeModel>
		{
			public RelatedRecipeRules()
			{
				RuleFor(model => model.Title)
				   .NotEmpty()
				   .WithMessage(model => model.Title, ValidationMessages.FieldRequired)
				   .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.Title, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

				RuleFor(model => model.Image)
					.NotEmpty()
					.WithMessage(model => model.Image, ValidationMessages.FieldRequired);

				RuleFor(model => model.Url)
					.Cascade(CascadeMode.StopOnFirstFailure)
					.NotEmpty()
					.WithMessage(model => model.Url, ValidationMessages.FieldRequired)
					.Matches(ValidationPatterns.ContentUrlPattern)
					.WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.Title, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
			}
		}
	}
}