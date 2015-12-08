using System;
using System.Threading.Tasks;
using Authorize.Net.AIM.Requests;
using Authorize.Net.AIM.Responses;
using Authorize.Net.Reporting;

namespace Authorize.Net.CIM
{
    public interface ICustomerGateway
    {
        Task<string> AddCreditCard(string profileId, string cardNumber, int expirationMonth, int expirationYear, string cardCode,
            Address billToAddress);

        Task<string> AddCreditCard(string profileId, string cardNumber, int expirationMonth, int expirationYear, string cardCode);

        Task<string> AddECheckBankAccount(string profileId, BankAccountType bankAccountType, string bankRoutingNumber,
            string bankAccountNumber, string personNameOnAccount);

        Task<string> AddECheckBankAccount(string profileId, BankAccountType bankAccountType, string bankRoutingNumber,
            string bankAccountNumber, string personNameOnAccount, string bankName, EcheckType eCheckType, Address billToAddress);

        Task<string> AddECheckBankAccount(string profileId, BankAccount bankAccount, Address billToAddress);

        Task<string> AddShippingAddress(string profileId, Address address);

        Task<string> AddShippingAddress(string profileId, string first, string last, string street, string city, string state, string zip,
            string country, string phone);

        Task<IGatewayResponse> AuthorizeAndCapture(string profileId, string paymentProfileId, decimal amount, decimal tax, decimal shipping);
        Task<IGatewayResponse> AuthorizeAndCapture(string profileId, string paymentProfileId, decimal amount);
        Task<IGatewayResponse> AuthorizeAndCapture(Order order);

        Task<IGatewayResponse> Authorize(string profileId, string paymentProfileId, decimal amount, decimal tax, decimal shipping);
        Task<IGatewayResponse> Authorize(string profileId, string paymentProfileId, decimal amount);
        Task<IGatewayResponse> Authorize(Order order);
        Task<IGatewayResponse> Capture(string profileId, string paymentProfileId, string cardCode, decimal amount, string approvalCode);

        [Obsolete("This method has been deprecated, instead use the overloaded method without the appoval code")]
        Task<IGatewayResponse> Refund(string profileId, string paymentProfileId, string approvalCode, string transactionId, decimal amount);

        [Obsolete("This method has been deprecated, instead use the overloaded method without the appoval code")]
        Task<IGatewayResponse> Void(string profileId, string paymentProfileId, string approvalCode, string transactionId);


        Task<IGatewayResponse> Refund(string profileId, string paymentProfileId, string transactionId, decimal amount);
        Task<IGatewayResponse> Void(string profileId, string paymentProfileId, string transactionId);

        Task<Customer> CreateCustomer(string email, string description);
        Task<bool> DeleteCustomer(string profileId);
        Task<bool> DeletePaymentProfile(string profileId, string paymentProfileId);
        Task<bool> DeleteShippingAddress(string profileId, string shippingAddressId);
        Task<Customer> GetCustomer(string profileId);
        Task<string[]> GetCustomerIDs();
        Task<Address> GetShippingAddress(string profileId, string shippingAddressId);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> UpdatePaymentProfile(string profileId, PaymentProfile profile);
        Task<bool> UpdateShippingAddress(string profileId, Address address);
        Task<string> ValidateProfile(string profileId, string paymentProfileId, string shippingAddressId, ValidationMode mode);
        Task<string> ValidateProfile(string profileId, string paymentProfileId, ValidationMode mode);
    }
}