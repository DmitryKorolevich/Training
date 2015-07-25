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
                .NotEmpty()
                .WithMessage(model => model.Text, ValidationMessages.FieldRequired)
				.Length(0, 1000)
				.WithMessage(model => model.Text, ValidationMessages.FieldLength,
					1000);
		}
	}
}