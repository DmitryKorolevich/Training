using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.ContentManagement
{
	public class RecipeVideoModelValidator : ModelValidator<VideoRecipeModel>
	{
		public override void Validate(VideoRecipeModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<VideoRecipeRules>().Validate(value));
		}

		public class VideoRecipeRules : AbstractValidator<VideoRecipeModel>
		{
			public VideoRecipeRules()
			{
				RuleFor(model => model.Text)
				   .NotEmpty()
				   .WithMessage(model => model.Text, ValidationMessages.FieldRequired)
				   .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.Text, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

				RuleFor(model => model.Image)
					.NotEmpty()
					.WithMessage(model => model.Image, ValidationMessages.FieldRequired);

				RuleFor(model => model.Video)
				   .NotEmpty()
				   .WithMessage(model => model.Video, ValidationMessages.FieldRequired)
				   .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
				   .WithMessage(model => model.Video, ValidationMessages.FieldLength,
					   BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
			}
		}
	}
}