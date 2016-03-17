using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
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

			if (value.Items.Count != ContentConstants.CONTENT_CROSS_SELL_LIMIT)
			{
				ParseResults(
					new ValidationResult(new List<ValidationFailure>()
					{
						new ValidationFailure(string.Empty, "Incorrect amount of cross sells")
					}));
			}

            var index = 0;
            foreach (var item in value.Items)
			{
				ParseResults(ValidatorsFactory.GetValidator<ContentCrossSellRules>().Validate(item), "Items", index);
                index++;
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
					.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
					.WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

				RuleFor(model => model.IdSku)
					.NotEqual(0)
					.WithMessage(model => model.IdSku, ValidationMessages.FieldRequired);
			}
		}
	}
}
