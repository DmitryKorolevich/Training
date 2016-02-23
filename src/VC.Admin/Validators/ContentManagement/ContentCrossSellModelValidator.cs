using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;


namespace VC.Admin.Validators.ContentManagement
{
	public class ContentCrossSellModelValidator : ModelValidator<ContentCrossSellModel>
	{
		public override void Validate(ContentCrossSellModel value)
		{
			ValidationErrors.Clear();
			foreach (var item in value.Items)
			{
				ParseResults(ValidatorsFactory.GetValidator<ContentCrossSellRules>().Validate(item));
			}
		}

		public class ContentCrossSellRules : AbstractValidator<ContentCrossSellItemModel>
		{
			public ContentCrossSellRules()
			{
				RuleFor(model => model.ImageUrl)
					.NotEmpty()
					.WithMessage(model => model.ImageUrl, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
					.WithMessage(model => model.ImageUrl, ValidationMessages.FieldLength,
						BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

				RuleFor(model => model.Title)
					.NotEmpty()
					.WithMessage(model => model.Title, ValidationMessages.FieldRequired)
					.Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
					.WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);

				RuleFor(model => model.IdSku)
					.NotNull()
					.WithMessage(model => model.IdSku, ValidationMessages.FieldRequired);

				RuleFor(model => model.Price)
					.NotEqual(0)
					.WithMessage(model => model.Price, ValidationMessages.FieldRequired);
			}
		}
	}
}
