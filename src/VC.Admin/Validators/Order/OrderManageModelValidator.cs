using System.Linq;
using FluentValidation;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Validation.Logic;
using VC.Admin.Validators.Customer;
using System;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VC.Admin.Models.Orders;
using VitalChoice.Infrastructure.Domain.Constants;

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
            var wireTransferPaymentModelValidator = ValidatorsFactory.GetValidator<WireTransferModelRules>();
            var marketingPaymentModelValidator = ValidatorsFactory.GetValidator<MarketingModelRules>();
            var VCWellnessPaymentModelValidator = ValidatorsFactory.GetValidator<VCWellnessModelRules>();
            var NCPaymentModelValidator = ValidatorsFactory.GetValidator<NCPaymentModelRules>();
            ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value, ruleSet: "Main"));
            if (value.UseShippingAndBillingFromCustomer)
            {
                ParseResults(ValidatorsFactory.GetValidator<OrderModelValidator>().Validate(value, ruleSet: "NewOrder"));
                if (value.Customer != null)
                {
                    ParseResults(customerValidator.Validate(value.Customer));
                    int index = 0;
                    foreach (var shipping in value.Customer.Shipping)
                    {
                        if (value.UpdateShippingAddressForCustomer && shipping.IsSelected && value.OrderStatus != OrderStatus.OnHold)
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
                            if (value.UpdateCardForCustomer && card.IsSelected && value.OrderStatus != OrderStatus.OnHold)
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
                    if (value.IdPaymentMethodType.Value == 6 && value.Customer.WireTransfer != null &&
                        (value.UpdateWireTransferForCustomer || value.OrderStatus != OrderStatus.OnHold))//wiretransfer
                    {
                        ParseResults(wireTransferPaymentModelValidator.Validate(value.Customer.WireTransfer), "wiretransfer");
                        ParseResults(addressValidator.Validate(value.Customer.WireTransfer.Address), "wiretransfer");
                    }
                    if (value.IdPaymentMethodType.Value == 7 && value.Customer.Marketing != null &&
                        (value.UpdateMarketingForCustomer || value.OrderStatus != OrderStatus.OnHold))//marketing
                    {
                        ParseResults(marketingPaymentModelValidator.Validate(value.Customer.Marketing), "marketing");
                        ParseResults(addressValidator.Validate(value.Customer.Marketing.Address), "marketing");
                    }
                    if (value.IdPaymentMethodType.Value == 8 && value.Customer.VCWellness != null &&
                        (value.UpdateVCWellnessForCustomer || value.OrderStatus != OrderStatus.OnHold))//vcwellness
                    {
                        ParseResults(VCWellnessPaymentModelValidator.Validate(value.Customer.VCWellness), "vcwellness");
                        ParseResults(addressValidator.Validate(value.Customer.VCWellness.Address), "vcwellness");
                    }
                    if (value.IdPaymentMethodType.Value == 4 && value.Customer.NC != null && value.OrderStatus != OrderStatus.OnHold)//nc 
                    {
                        ParseResults(NCPaymentModelValidator.Validate(value.Customer.NC), "nc");
                        ParseResults(addressValidator.Validate(value.Customer.NC.Address), "nc");
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
                    if (value.IdPaymentMethodType.Value == 6 && value.WireTransfer != null)//wiretransfer
                    {
                        ParseResults(wireTransferPaymentModelValidator.Validate(value.WireTransfer), "wiretransfer");
                        ParseResults(addressValidator.Validate(value.WireTransfer.Address), "wiretransfer");
                    }
                    if (value.IdPaymentMethodType.Value == 7 && value.Marketing != null)//marketing
                    {
                        ParseResults(marketingPaymentModelValidator.Validate(value.Marketing), "marketing");
                        ParseResults(addressValidator.Validate(value.Marketing.Address), "marketing");
                    }
                    if (value.IdPaymentMethodType.Value == 8 && value.VCWellness != null)//vcwellness
                    {
                        ParseResults(VCWellnessPaymentModelValidator.Validate(value.VCWellness), "vcwellness");
                        ParseResults(addressValidator.Validate(value.VCWellness.Address), "vcwellness");
                    }
                    if (value.IdPaymentMethodType.Value == 4 && value.NC != null)//nc
                    {
                        ParseResults(NCPaymentModelValidator.Validate(value.NC), "nc");
                        ParseResults(addressValidator.Validate(value.NC.Address), "nc");
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
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldRequired)
                               .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);
                             RuleFor(model => model.PoNumber)
                               .NotEmpty()
                               .When(p => p.Customer != null && p.Customer.CustomerType == CustomerType.Wholesale && p.IdObjectType!=(int)OrderType.GiftList)
                               .WithMessage(model => model.PoNumber, ValidationMessages.FieldRequired)
                               .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)
                               .WithMessage(model => model.PoNumber, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE);

                             RuleFor(model => model.OrderNotes)
                              .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                              .WithMessage(model => model.OrderNotes, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                             RuleFor(model => model.GiftMessage)
                                 .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                                 .WithMessage(model => model.GiftMessage, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                             
                             RuleFor(model => model.ShipDelayDate)
                              .Must(p => p.HasValue)
                              .When(p => p.ShipDelayType == ShipDelayType.EntireOrder)
                              .WithMessage(model => model.ShipDelayDate, ValidationMessages.FieldRequired);
                            RuleFor(model => model.ShipDelayDate)
                              .Must(p=>p >= DateTime.Now)
                              .When(p => p.ShipDelayDate.HasValue && p.ShipDelayType == ShipDelayType.EntireOrder)
                              .WithMessage("Ship delay should be future date. Please review.");
                             RuleFor(model => model.ShipDelayDateP)
                                 .Must(p => p >= DateTime.Now)
                                 .When(p => p.ShipDelayDateP.HasValue && p.ShipDelayType == ShipDelayType.PerishableAndNonPerishable)
                                 .WithMessage("Ship delay should be future date. Please review.");
                             RuleFor(model => model.ShipDelayDateNP)
                                 .Must(p => p >= DateTime.Now)
                                 .When(p => p.ShipDelayDateNP.HasValue && p.ShipDelayType== ShipDelayType.PerishableAndNonPerishable)
                                 .WithMessage("Ship delay should be future date. Please review.");

                            RuleFor(model => model.OrderNotes)
                               .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                             RuleFor(model => model.ServiceCodeNotes)
                               .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
                             RuleFor(model => model.GiftMessage)
                               .Length(0, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)
                               .WithMessage(model => model.KeyCode, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE);
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
                               .Must(p => p.Any(x => x.IsSelected))
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