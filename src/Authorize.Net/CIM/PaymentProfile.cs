using System;
using Authorize.Net.AIM.Requests;
using Authorize.Net.Reporting;
using Authorize.Net.Utility;

namespace Authorize.Net.CIM
{
    /// <summary>
    ///     An abstraction for the AuthNET API, allowing you store credit card information with Authorize.net
    /// </summary>
    public class PaymentProfile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PaymentProfile" /> class, using the passed-in API type to create the
        ///     profile.
        /// </summary>
        /// <param name="apiType">Type of the API.</param>
        public PaymentProfile(customerPaymentProfileMaskedType apiType)
        {
            if (apiType.billTo != null)
                BillingAddress = new Address(apiType.billTo);

            ProfileID = apiType.customerPaymentProfileId;

            if (apiType.driversLicense != null)
            {
                DriversLicenseNumber = apiType.driversLicense.number;
                DriversLicenseState = apiType.driversLicense.state;
                DriversLicenseDOB = apiType.driversLicense.dateOfBirth;
            }

            if (apiType.customerTypeSpecified)
            {
                IsBusiness = apiType.customerType == customerTypeEnum.business;
            }
            else
            {
                IsBusiness = false;
            }

            if (apiType.payment != null)
            {
                if (apiType.payment.Item is creditCardMaskedType)
                {
                    var card = (creditCardMaskedType) apiType.payment.Item;
                    CardType = card.cardType;
                    CardNumber = card.cardNumber;
                    CardExpiration = card.expirationDate;
                }
                else if (apiType.payment.Item is bankAccountMaskedType)
                {
                    var bankAcct = (bankAccountMaskedType) apiType.payment.Item;
                    eCheckBankAccount = new BankAccount
                    {
                        accountTypeSpecified = bankAcct.accountTypeSpecified,
                        accountType = (BankAccountType) Enum.Parse(typeof (BankAccountType), bankAcct.accountType.ToString(), true),
                        routingNumber = bankAcct.routingNumber,
                        accountNumber = bankAcct.accountNumber,
                        nameOnAccount = bankAcct.nameOnAccount,
                        echeckTypeSpecified = bankAcct.echeckTypeSpecified,
                        echeckType = (EcheckType) Enum.Parse(typeof (EcheckType), bankAcct.echeckType.ToString(), true),
                        bankName = bankAcct.bankName,
                        checkNumber = ""
                    };
                }
            }
            TaxID = apiType.taxId;
        }

        public Address BillingAddress { get; set; }
        public string ProfileID { get; set; }
        public bool IsBusiness { get; set; }

        public string DriversLicenseNumber { get; set; }
        public string DriversLicenseDOB { get; set; }
        public string DriversLicenseState { get; set; }

        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string CardExpiration { get; set; }
        public string CardCode { get; set; }
        public string TaxID { get; set; }

        // eCheck
        public BankAccount eCheckBankAccount { get; set; }


        /// <summary>
        ///     Creates an API object, ready to send to AuthNET servers.
        /// </summary>
        /// <returns></returns>
        public customerPaymentProfileExType ToAPI()
        {
            var result = new customerPaymentProfileExType();

            if (null != BillingAddress)
            {
                result.billTo = BillingAddress.ToAPIType();
            }

            result.customerPaymentProfileId = ProfileID;

            if (!string.IsNullOrEmpty(DriversLicenseNumber))
            {
                result.driversLicense = new driversLicenseType
                {
                    dateOfBirth = DriversLicenseDOB,
                    number = DriversLicenseNumber,
                    state = DriversLicenseState
                };
            }

            if (IsBusiness)
            {
                result.customerType = customerTypeEnum.business;
            }
            else
            {
                result.customerType = customerTypeEnum.individual;
            }
            result.customerTypeSpecified = true;

            result.payment = new paymentType();
            if (!string.IsNullOrWhiteSpace(CardNumber))
            {
                var card = new creditCardType
                {
                    cardCode = CardCode,
                    cardNumber = CardNumber,
                    expirationDate = CardExpiration
                };
                result.payment.Item = card;
            }
            else if (!string.IsNullOrWhiteSpace(eCheckBankAccount?.accountNumber))
            {
                var bankAccount = new bankAccountType
                {
                    accountTypeSpecified = eCheckBankAccount.accountTypeSpecified,
                    accountType =
                        (bankAccountTypeEnum) Enum.Parse(typeof (bankAccountTypeEnum), eCheckBankAccount.accountType.ToString(), true),
                    routingNumber = eCheckBankAccount.routingNumber,
                    accountNumber = eCheckBankAccount.accountNumber,
                    nameOnAccount = eCheckBankAccount.nameOnAccount,
                    echeckTypeSpecified = eCheckBankAccount.echeckTypeSpecified,
                    echeckType = (echeckTypeEnum) Enum.Parse(typeof (echeckTypeEnum), eCheckBankAccount.echeckType.ToString(), true),
                    bankName = eCheckBankAccount.bankName,
                    checkNumber = eCheckBankAccount.checkNumber
                };
                result.payment.Item = bankAccount;
            }

            if (!string.IsNullOrEmpty(TaxID))
            {
                result.taxId = TaxID;
            }
            return result;
        }
    }

    public class ProfileAmountType
    {
        public string ID { get; set; }
    }
}