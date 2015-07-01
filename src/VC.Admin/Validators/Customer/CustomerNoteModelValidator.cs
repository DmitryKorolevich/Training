using FluentValidation;
using VC.Admin.Models.Customer;
using VitalChoice.Validation.Logic;

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
			
		}
	}
}