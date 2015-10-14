using System.Linq;
using FluentValidation;
using VC.Admin.Models.Product;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Domain.Constants;
using VC.Admin.Models.Order;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using System;
using System.Globalization;

namespace VC.Admin.Validators.Order
{
    public class OrderManageModelValidator : ModelValidator<OrderManageModel>
    {
        public override void Validate(OrderManageModel value)
        {
            ValidationErrors.Clear();
            var addressValidator = ValidatorsFactory.GetValidator<AddressModelRules>();
            var creditCardValidator = ValidatorsFactory.GetValidator<CreditCardModelRules>();
            var checkPaymentModelValidator = ValidatorsFactory.GetValidator<CheckPaymentModelRules>();
            var oacPaymentModelValidator = ValidatorsFactory.GetValidator<OacPaymentModelRules>();
            var customerValidator = ValidatorsFactory.GetValidator<CustomerAddUpdateModelValidator.CustomerModelRules>();
            ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value, ruleSet: "Main"));
            if (value.Id == 0)
            {
                ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value, ruleSet: "NewOrder"));
                if (value.Customer != null)
                {
                    ParseResults(customerValidator.Validate(value.Customer));
                    int index = 0;
                    foreach (var shipping in value.Customer.Shipping)
                    {
                        if (value.UpdateShippingAddressForCustomer || (shipping.IsSelected && value.OrderStatus != OrderStatus.OnHold))
                        {
                            ParseResults(addressValidator.Validate(shipping), "Shipping", index, "shipping");
                        }
                        index++;
                    }
                    if (value.IdPaymentMethodType.Value == 1)//card
                    {
                        index = 0;
                        foreach (var card in value.Customer.CreditCards)
                        {
                            if (value.UpdateCardForCustomer || (card.IsSelected && value.OrderStatus != OrderStatus.OnHold))
                            {
                                ParseResults(creditCardValidator.Validate(card), "CreditCards", index, "card");
                                ParseResults(addressValidator.Validate(card.Address), "CreditCards", index, "card");
                            }
                            index++;
                        }
                    }
                    if (value.IdPaymentMethodType.Value == 2 && value.Customer.Oac != null &&
                        (value.UpdateOACForCustomer || value.OrderStatus != OrderStatus.OnHold))//oac
                    {
                        ParseResults(oacPaymentModelValidator.Validate(value.Customer.Oac), "oac");
                        ParseResults(addressValidator.Validate(value.Customer.Oac.Address), "oac");
                    }
                    if (value.IdPaymentMethodType.Value == 3 && value.Customer.Check != null &&
                        (value.UpdateCheckForCustomer || value.OrderStatus != OrderStatus.OnHold))//check
                    {
                        ParseResults(checkPaymentModelValidator.Validate(value.Customer.Check), "check");
                        ParseResults(addressValidator.Validate(value.Customer.Check.Address), "check");
                    }
                }
            }
            else
            {
                ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value, ruleSet: "ExistOrder"));
                ParseResults(addressValidator.Validate(value.Shipping), "shipping");
                if (value.IdPaymentMethodType.HasValue)
                {
                    if (value.IdPaymentMethodType.Value == 1 && value.CreditCard != null)//card
                    {
                        ParseResults(creditCardValidator.Validate(value.CreditCard), "card");
                        ParseResults(addressValidator.Validate(value.CreditCard.Address), "card");
                    }
                    if (value.IdPaymentMethodType.Value == 2 && value.Oac != null)//oac
                    {
                        ParseResults(oacPaymentModelValidator.Validate(value.Oac), "oac");
                        ParseResults(addressValidator.Validate(value.Oac.Address), "oac");
                    }
                    if (value.IdPaymentMethodType.Value == 3 && value.Check != null)//check
                    {
                        ParseResults(checkPaymentModelValidator.Validate(value.Check), "check");
                        ParseResults(addressValidator.Validate(value.Check.Address), "check");
                    }
                }
            }
        }

        private class OrderModelValidator : AbstractValidator<OrderManageModel>
        {
            public OrderModelValidator()
            {
                RuleSet("Main",
                         () =>
                         {
                             RuleFor(model => model.IdPaymentMethodType)
                                .Must(p => p.HasValue)
                                .WithMessage(model => model.IdPaymentMethodType, ValidationMessages.FieldRequired);
                             RuleFor(model => model.KeyCode)
                               .NotEmpty()
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldRequired);
                             RuleFor(model => model.PoNumber)
                               .NotEmpty()
                               .When(p => p.Customer != null && p.Customer.CustomerType == CustomerType.Wholesale)
                               .WithMessage(model => model.PoNumber, ValidationMessages.FieldRequired);

                             RuleFor(model => model.ShipDelayDate)
                              .Must(p => p.HasValue)
                              .When(p => p.ShipDelayType == 1)
                              .WithMessage(model => model.ShipDelayDate, ValidationMessages.FieldRequired);
                            RuleFor(model => model.ShipDelayDate)
                              .Must(p=>p >= DateTime.Now)
                              .When(p => p.ShipDelayDate.HasValue)
                              .WithMessage(model => model.ShipDelayDate, ValidationMessages.FieldMinOrEqual, DateTime.Now.AddDays(1).ToString("d",CultureInfo.InvariantCulture));
                             RuleFor(model => model.ShipDelayDateP)
                                 .Must(p => p >= DateTime.Now)
                                 .When(p => p.ShipDelayDateP.HasValue)
                                 .WithMessage(model => model.ShipDelayDateP, ValidationMessages.FieldMinOrEqual, DateTime.Now.AddDays(1).ToString("d", CultureInfo.InvariantCulture));
                             RuleFor(model => model.ShipDelayDateNP)
                                 .Must(p => p >= DateTime.Now)
                                 .When(p => p.ShipDelayDateNP.HasValue)
                                 .WithMessage(model => model.ShipDelayDateNP, ValidationMessages.FieldMinOrEqual, DateTime.Now.AddDays(1).ToString("d", CultureInfo.InvariantCulture));
                         });
                RuleSet("NewOrder",
                        () =>
                        {
                            RuleFor(model => model.Customer.Source)
                               .Must(p=>p.HasValue)
                               .When(p => p.Customer != null && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Customer.Source, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Customer)
                               .Must(p => p != null)
                               .WithMessage(model => model.Customer, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Customer.CreditCards)
                               .Must(p => p.Where(x => x.IsSelected).Any())
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 1 && p.Customer != null && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Customer.CreditCards, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Customer.Oac)
                               .Must(p => p != null)
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 2 && p.Customer != null && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Customer.Oac, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Customer.Check)
                               .Must(p => p != null)
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 3 && p.Customer != null && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Customer.Check, ValidationMessages.FieldRequired);

                        });
                RuleSet("ExistOrder",
                        () =>
                        {
                            RuleFor(model => model.Shipping)
                               .Must(p => p != null)
                               .WithMessage(model => model.Shipping, ValidationMessages.FieldRequired);
                            RuleFor(model => model.CreditCard)
                               .Must(p => p != null)
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 1 && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.CreditCard, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Oac)
                               .Must(p => p != null)
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 2 && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Oac, ValidationMessages.FieldRequired);
                            RuleFor(model => model.Check)
                               .Must(p => p != null)
                               .When(p => p.IdPaymentMethodType.HasValue && p.IdPaymentMethodType.Value == 3 && p.OrderStatus != OrderStatus.OnHold)
                               .WithMessage(model => model.Check, ValidationMessages.FieldRequired);
                        });
            }
        }
    }
}