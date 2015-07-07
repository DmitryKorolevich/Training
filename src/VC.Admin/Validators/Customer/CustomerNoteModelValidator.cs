using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Core.Infrastructure.Helpers;

namespace VC.Admin.Validators.Customer
{
	public class CustomerNoteModelValidator : ModelValidator<CustomerNoteModel>
	{
		public override void Validate(CustomerNoteModel value)
		{
			ValidationErrors.Clear();
			ParseResults(ValidatorsFactory.GetValidator<CustomerNoteModelRules>().Validate(value));
		}
	}

	public class CustomerNoteModelRules : AbstractValidator<CustomerNoteModel>
	{
		public CustomerNoteModelRules()
		{
			RuleFor(model => model.Text)
				.Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
				.WithMessage(model => model.Text, ValidationMessages.FieldLength,
					BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
		}
	}
}