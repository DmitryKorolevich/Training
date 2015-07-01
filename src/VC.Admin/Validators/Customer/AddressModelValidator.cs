using FluentValidation;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;

namespace VC.Admin.Validators.Customer
{
    public class AddressModelValidator : ModelValidator<AddressModel>
    {
        public override void Validate(AddressModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<AddressModelRules>().Validate(value));
        }
    }

	public class AddressModelRules : AbstractValidator<AddressModel>
	{
		public AddressModelRules()
		{
			
		}
	}
}