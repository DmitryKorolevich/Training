using System;
using Authorize.Net.CIM;
using Authorize.Net.Reporting;
using Authorize.Net.Utility;

namespace Authorize.Net.ARB
{
    public interface ISubscriptionRequest
    {
        decimal Amount { get; set; }
        Address BillingAddress { get; set; }
        short BillingCycles { get; set; }
        short BillingInterval { get; set; }
        BillingIntervalUnits BillingIntervalUnits { get; set; }
        string CardCode { get; set; }
        int CardExpirationMonth { get; set; }
        int CardExpirationYear { get; set; }
        string CardNumber { get; set; }
        BankAccount eCheckBankAccount { get; set; }
        string CustomerEmail { get; set; }
        string CustomerID { get; set; }
        Address ShippingAddress { get; set; }
        DateTime StartsOn { get; set; }
        string SubscriptionID { get; set; }
        string SubscriptionName { get; set; }
        string Description { get; set; }
        string Invoice { get; set; }
        decimal TrialAmount { get; set; }
        short TrialBillingCycles { get; set; }
        SubscriptionRequest SetTrialPeriod(short trialBillingCycles, decimal trialAmount);
        ARBSubscriptionType ToAPI();
        ARBSubscriptionType ToUpdateableAPI();

        SubscriptionRequest UsingCreditCard(string firstName, string lastName, string cardNumber, int cardExpirationYear,
            int cardExpirationMonth);

        SubscriptionRequest WithBillingAddress(Address add);
        SubscriptionRequest WithShippingAddress(Address add);
    }
}