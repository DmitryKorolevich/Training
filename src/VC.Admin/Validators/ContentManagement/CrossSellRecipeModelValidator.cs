using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class CrossSellRecipeModelValidator : ModelValidator<CrossSellRecipeModel>
    {
        public override void Validate(CrossSellRecipeModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<CrossSellRecipeRules>().Validate(value));
        }

        public class CrossSellRecipeRules : AbstractValidator<CrossSellRecipeModel>
        {
            public CrossSellRecipeRules()
            {
                RuleFor(model => model.Subtitle)
                    .NotEmpty()
                    .WithMessage(model => model.Subtitle, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
					.WithMessage(model => model.Subtitle, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

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