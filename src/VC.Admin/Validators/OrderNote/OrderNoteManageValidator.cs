using FluentValidation;
using VC.Admin.Models.OrderNote;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;

namespace VC.Admin.Validators.OrderNote
{
    public class OrderNoteManageValidator : ModelValidator<ManageOrderNoteModel>
    {
		public override void Validate(ManageOrderNoteModel value)
		{
			ValidationErrors.Clear();

			var validator = ValidatorsFactory.GetValidator<OrderNoteRuleSet>();
            ParseResults(validator.Validate(value));
		}

		private class OrderNoteRuleSet :  AbstractValidator<ManageOrderNoteModel>
		{
			public OrderNoteRuleSet()
			{

				RuleFor(model => model.Title)
					.NotEmpty()
					.WithMessage(model => model.Title, ValidationMessages.FieldRequired);
				RuleFor(model => model.Title)
					.Length(0, 50)
					.WithMessage(model => model.Title, ValidationMessages.FieldLength, 50);

				RuleFor(model => model.Description)
					.NotEmpty()
					.WithMessage(model => model.Description, ValidationMessages.FieldRequired);
				RuleFor(model => model.Description)
					.Length(0, 1000)
					.WithMessage(model => model.Description, ValidationMessages.FieldLength, 1000);
			}
		}
    }
}