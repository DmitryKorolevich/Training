using System;
using System.Threading.Tasks;
using Authorize.Net.AIM;
using Authorize.Net.AIM.Requests;
using Authorize.Net.AIM.Responses;
using Authorize.Net.Reporting;
using Authorize.Net.Utility;

namespace Authorize.Net.CIM
{
    public class CustomerGateway : ICustomerGateway
    {
        private readonly HttpXmlUtility _gateway;
        private readonly validationModeEnum _mode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomerGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        /// <param name="mode">Test or Live.</param>
        public CustomerGateway(string apiLogin, string transactionKey, ServiceMode mode)
        {
            if (mode == ServiceMode.Live)
            {
                _gateway = new HttpXmlUtility(ServiceMode.Live, apiLogin, transactionKey);
                _mode = validationModeEnum.liveMode;
            }
            else
            {
                _gateway = new HttpXmlUtility(ServiceMode.Test, apiLogin, transactionKey);
                _mode = validationModeEnum.testMode;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomerGateway" /> class.
        /// </summary>
        /// <param name="apiLogin">The API login.</param>
        /// <param name="transactionKey">The transaction key.</param>
        public CustomerGateway(string apiLogin, string transactionKey) : this(apiLogin, transactionKey, ServiceMode.Test)
        {
        }

        public Task<Customer> CreateCustomer(string email, string description)
        {
            return CreateCustomer(email, description, "");
        }

        /// <summary>
        ///     Retrieve an existing customer profile along with all the associated customer payment profiles and customer shipping
        ///     addresses.
        /// </summary>
        /// <param name="profileId">The profile ID</param>
        /// <returns></returns>
        public async Task<Customer> GetCustomer(string profileId)
        {
            var req = new getCustomerProfileRequest {customerProfileId = profileId};


            var response = (getCustomerProfileResponse) await _gateway.Send(req);


            var result = new Customer
            {
                Email = response.profile.email,
                Description = response.profile.description,
                ProfileID = response.profile.customerProfileId,
                ID = response.profile.merchantCustomerId
            };


            if (response.profile.shipToList != null)
            {
                foreach (var address in response.profile.shipToList)
                {
                    result.ShippingAddresses.Add(new Address(address));
                }
            }

            if (response.profile.paymentProfiles != null)
            {
                foreach (var paymentProfile in response.profile.paymentProfiles)
                {
                    result.PaymentProfiles.Add(new PaymentProfile(paymentProfile));
                }
            }
            return result;
        }

        /// <summary>
        ///     Adds a credit card profile to the user and returns the profile ID
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <param name="cardCode">The card code.</param>
        /// <returns></returns>
        public Task<string> AddCreditCard(string profileId, string cardNumber, int expirationMonth, int expirationYear, string cardCode)
        {
            return AddCreditCard(profileId, cardNumber, expirationMonth, expirationYear, cardCode, null);
        }

        /// <summary>
        ///     Adds a credit card profile to the user and returns the profile ID
        /// </summary>
        /// <returns></returns>
        public async Task<string> AddCreditCard(string profileId, string cardNumber, int expirationMonth, int expirationYear,
            string cardCode, Address billToAddress)
        {
            // Get the expiration date.

            DateTime dt; // = DateTime.Parse(expirationMonth.ToString() + "-1-" + expirationYear.ToString());
            if (!CommonFunctions.ParseDateTime(expirationYear, expirationMonth, 1, out dt))
            {
                throw new Exception("Invalid credit card expiration date");
            }

            var expDate = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
            var sExpDate = expDate.ToString("yyyy-MM");
            // Make sure the card has not expired.
            if (expDate <= DateTime.Now)
                throw new Exception("The credit card expiration date \"" + sExpDate + "\" is expired.");

            var req = new createCustomerPaymentProfileRequest
            {
                customerProfileId = profileId,
                paymentProfile = new customerPaymentProfileType {payment = new paymentType()}
            };

            var card = new creditCardType();
            if (!string.IsNullOrEmpty(cardCode)) card.cardCode = cardCode;
            card.cardNumber = cardNumber;
            card.expirationDate = sExpDate;
            req.paymentProfile.payment.Item = card;

            if (billToAddress != null)
                req.paymentProfile.billTo = billToAddress.ToAPIType();

            req.validationModeSpecified = true;
            req.validationMode = _mode;

            var response = (createCustomerPaymentProfileResponse) await _gateway.Send(req);

            return response.customerPaymentProfileId;
        }

        /// <summary>
        ///     Adds a eCheck bank account profile to the user and returns the profile ID
        /// </summary>
        /// <returns></returns>
        public Task<string> AddECheckBankAccount(string profileId, BankAccountType bankAccountType, string bankRoutingNumber,
            string bankAccountNumber, string personNameOnAccount)
        {
            return AddECheckBankAccount(profileId,
                new BankAccount
                {
                    accountTypeSpecified = true,
                    accountType = bankAccountType,
                    routingNumber = bankRoutingNumber,
                    accountNumber = bankAccountNumber,
                    nameOnAccount = personNameOnAccount
                }, null);
        }

        /// <summary>
        ///     Adds a bank account profile to the user and returns the profile ID
        /// </summary>
        /// <returns></returns>
        public Task<string> AddECheckBankAccount(string profileId, BankAccountType bankAccountType, string bankRoutingNumber,
            string bankAccountNumber,
            string personNameOnAccount, string bankName, EcheckType eCheckType,
            Address billToAddress)
        {
            return AddECheckBankAccount(profileId,
                new BankAccount
                {
                    accountTypeSpecified = true,
                    accountType = bankAccountType,
                    routingNumber = bankRoutingNumber,
                    accountNumber = bankAccountNumber,
                    nameOnAccount = personNameOnAccount,
                    bankName = bankName,
                    echeckTypeSpecified = true,
                    echeckType = eCheckType
                }, billToAddress);
        }

        /// <summary>
        ///     Adds a bank account profile to the user and returns the profile ID
        /// </summary>
        /// <returns></returns>
        public async Task<string> AddECheckBankAccount(string profileId, BankAccount bankAccount, Address billToAddress)
        {
            var req = new createCustomerPaymentProfileRequest
            {
                customerProfileId = profileId,
                paymentProfile = new customerPaymentProfileType {payment = new paymentType()}
            };


            var bankAcct = new bankAccountType
            {
                accountTypeSpecified = bankAccount.accountTypeSpecified,
                accountType = (bankAccountTypeEnum) Enum.Parse(typeof (bankAccountTypeEnum), bankAccount.accountType.ToString(), true),
                routingNumber = bankAccount.routingNumber,
                accountNumber = bankAccount.accountNumber,
                nameOnAccount = bankAccount.nameOnAccount,
                bankName = bankAccount.bankName,
                echeckTypeSpecified = bankAccount.echeckTypeSpecified,
                echeckType = (echeckTypeEnum) Enum.Parse(typeof (echeckTypeEnum), bankAccount.echeckType.ToString(), true)
            };

            req.paymentProfile.payment.Item = bankAcct;

            if (billToAddress != null)
                req.paymentProfile.billTo = billToAddress.ToAPIType();

            req.validationModeSpecified = true;
            req.validationMode = _mode;

            var response = (createCustomerPaymentProfileResponse) await _gateway.Send(req);

            return response.customerPaymentProfileId;
        }

        /// <summary>
        ///     Adds a Shipping Address to the customer profile
        /// </summary>
        public Task<string> AddShippingAddress(string profileId, string first, string last, string street, string city, string state,
            string zip, string country, string phone)
        {
            return AddShippingAddress(profileId,
                new Address
                {
                    First = first,
                    Last = last,
                    City = city,
                    State = state,
                    Country = country,
                    Zip = zip,
                    Phone = phone,
                    Street = street
                });
        }

        /// <summary>
        ///     Adds a Shipping Address to the customer profile
        /// </summary>
        public async Task<string> AddShippingAddress(string profileId, Address address)
        {
            var req = new createCustomerShippingAddressRequest
            {
                address = address.ToAPIType(),
                customerProfileId = profileId
            };

            var response = (createCustomerShippingAddressResponse) await _gateway.Send(req);

            return response.customerAddressId;
        }

        /// <summary>
        ///     Authorizes and Captures a transaction using the supplied profile information.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> AuthorizeAndCapture(string profileId, string paymentProfileId, decimal amount)
        {
            return AuthorizeAndCapture(profileId, paymentProfileId, amount, 0, 0);
        }


        /// <summary>
        ///     Authorizes and Captures a transaction using the supplied profile information with Tax and Shipping specified. If
        ///     you want
        ///     to add more detail here, use the 3rd option - which is to add an Order object
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="tax">The tax.</param>
        /// <param name="shipping">The shipping.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> AuthorizeAndCapture(string profileId, string paymentProfileId, decimal amount, decimal tax,
            decimal shipping)
        {
            var order = new Order(profileId, paymentProfileId, "");
            order.Amount = amount;

            if (tax > 0)
            {
                order.SalesTaxAmount = tax;
                order.SalesTaxName = "Sales Tax";
            }

            if (shipping > 0)
            {
                order.ShippingAmount = shipping;
                order.ShippingName = "Shipping";
            }

            return AuthorizeAndCapture(order);
        }

        /// <summary>
        ///     Authorizes and Captures a transaction using the supplied profile information, abstracted through an Order object.
        ///     Using the Order
        ///     you can add line items, specify shipping and tax, etc.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public async Task<IGatewayResponse> AuthorizeAndCapture(Order order)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransAuthCaptureType();

            trans.customerProfileId = order.CustomerProfileID;
            trans.customerPaymentProfileId = order.PaymentProfileID;
            trans.amount = order.Total;

            if (!string.IsNullOrEmpty(order.ShippingAddressProfileID))
            {
                trans.customerShippingAddressId = order.ShippingAddressProfileID;
            }

            if (order.SalesTaxAmount > 0)
                trans.tax = new extendedAmountType
                {
                    amount = order.SalesTaxAmount,
                    description = order.SalesTaxName,
                    name = order.SalesTaxName
                };

            if (order.ShippingAmount > 0)
                trans.shipping = new extendedAmountType
                {
                    amount = order.ShippingAmount,
                    description = order.ShippingName,
                    name = order.ShippingName
                };

            //line items
            if (order._lineItems.Count > 0)
            {
                trans.lineItems = order._lineItems.ToArray();
            }

            if (order.TaxExempt.HasValue)
            {
                trans.taxExempt = order.TaxExempt.Value;
                trans.taxExemptSpecified = true;
            }

            if (order.RecurringBilling.HasValue)
            {
                trans.recurringBilling = order.RecurringBilling.Value;
                trans.recurringBillingSpecified = true;
            }
            if (!string.IsNullOrEmpty(order.CardCode))
                trans.cardCode = order.CardCode;

            if (!string.IsNullOrEmpty(order.InvoiceNumber) ||
                !string.IsNullOrEmpty(order.Description) ||
                !string.IsNullOrEmpty(order.PONumber))
            {
                trans.order = new orderExType();
                if (!string.IsNullOrEmpty(order.InvoiceNumber))
                    trans.order.invoiceNumber = order.InvoiceNumber;
                if (!string.IsNullOrEmpty(order.Description))
                    trans.order.description = order.Description;
                if (!string.IsNullOrEmpty(order.PONumber))
                    trans.order.purchaseOrderNumber = order.PONumber;
            }

            req.transaction = new profileTransactionType();
            req.transaction.Item = trans;
            req.extraOptions = order.ExtraOptions;

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);


            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Authorizes a transaction using the supplied profile information.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> Authorize(string profileId, string paymentProfileId, decimal amount)
        {
            return Authorize(profileId, paymentProfileId, amount, 0, 0);
        }


        /// <summary>
        ///     Authorizes a transaction using the supplied profile information with Tax and Shipping specified. If you want
        ///     to add more detail here, use the 3rd option - which is to add an Order object
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="tax">The tax.</param>
        /// <param name="shipping">The shipping.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> Authorize(string profileId, string paymentProfileId, decimal amount, decimal tax, decimal shipping)
        {
            var order = new Order(profileId, paymentProfileId, "");
            order.Amount = amount;

            if (tax > 0)
            {
                order.SalesTaxAmount = tax;
                order.SalesTaxName = "Sales Tax";
            }

            if (shipping > 0)
            {
                order.ShippingAmount = shipping;
                order.ShippingName = "Shipping";
            }

            return Authorize(order);
        }

        /// <summary>
        ///     Authorizes a transaction using the supplied profile information, abstracted through an Order object. Using the
        ///     Order
        ///     you can add line items, specify shipping and tax, etc.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns>A string representing the approval code</returns>
        public async Task<IGatewayResponse> Authorize(Order order)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransAuthOnlyType
            {
                customerProfileId = order.CustomerProfileID,
                customerPaymentProfileId = order.PaymentProfileID,
                amount = order.Total,
                order = new orderExType
                {
                    description = order.Description,
                    invoiceNumber = order.InvoiceNumber,
                    purchaseOrderNumber = order.PONumber
                }
            };


            //order information

            if (!string.IsNullOrEmpty(order.ShippingAddressProfileID))
            {
                trans.customerShippingAddressId = order.ShippingAddressProfileID;
            }

            if (order.SalesTaxAmount > 0)
                trans.tax = new extendedAmountType
                {
                    amount = order.SalesTaxAmount,
                    description = order.SalesTaxName,
                    name = order.SalesTaxName
                };

            if (order.ShippingAmount > 0)
                trans.shipping = new extendedAmountType
                {
                    amount = order.ShippingAmount,
                    description = order.ShippingName,
                    name = order.ShippingName
                };

            //line items
            if (order._lineItems.Count > 0)
            {
                trans.lineItems = order._lineItems.ToArray();
            }

            if (order.TaxExempt.HasValue)
            {
                trans.taxExempt = order.TaxExempt.Value;
                trans.taxExemptSpecified = true;
            }

            if (order.RecurringBilling.HasValue)
            {
                trans.recurringBilling = order.RecurringBilling.Value;
                trans.recurringBillingSpecified = true;
            }
            if (!string.IsNullOrEmpty(order.CardCode))
                trans.cardCode = order.CardCode;

            req.transaction = new profileTransactionType {Item = trans};
            req.extraOptions = order.ExtraOptions;

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);

            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Captures the specified transaction.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="cardCode">The 3 or 4 digit card code in the signature space.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="approvalCode">The approval code.</param>
        /// <returns></returns>
        public async Task<IGatewayResponse> Capture(string profileId, string paymentProfileId, string cardCode, decimal amount,
            string approvalCode)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransCaptureOnlyType
            {
                approvalCode = approvalCode,
                customerProfileId = profileId,
                amount = amount
            };
            if (!string.IsNullOrEmpty(cardCode)) trans.cardCode = cardCode;
            trans.customerPaymentProfileId = paymentProfileId;

            req.transaction = new profileTransactionType();
            req.transaction.Item = trans;

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);
            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Refunds a transaction for the specified amount
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="approvalCode">The approval code.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        [Obsolete("This method has been deprecated, instead use the overloaded method without the appoval code")]
        public Task<IGatewayResponse> Refund(string profileId, string paymentProfileId, string transactionId, string approvalCode,
            decimal amount)
        {
            return Refund(profileId, paymentProfileId, transactionId, amount);
        }

        /// <summary>
        ///     Refunds a transaction for the specified amount
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public async Task<IGatewayResponse> Refund(string profileId, string paymentProfileId, string transactionId, decimal amount)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransRefundType
            {
                amount = amount,
                customerProfileId = profileId,
                customerPaymentProfileId = paymentProfileId,
                transId = transactionId
            };

            req.transaction = new profileTransactionType {Item = trans};

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);
            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Voids a previously authorized transaction
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="approvalCode">The approval code.</param>
        /// <returns></returns>
        [Obsolete("This method has been deprecated, instead use the overloaded method without the appoval code")]
        public Task<IGatewayResponse> Void(string profileId, string paymentProfileId, string transactionId, string approvalCode)
        {
            return Void(profileId, paymentProfileId, transactionId);
        }

        /// <summary>
        ///     Voids a previously authorized transaction
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        public async Task<IGatewayResponse> Void(string profileId, string paymentProfileId, string transactionId)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransVoidType
            {
                customerProfileId = profileId,
                customerPaymentProfileId = paymentProfileId,
                transId = transactionId
            };

            req.transaction = new profileTransactionType {Item = trans};

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);
            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Deletes a customer from the AuthNET servers.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <returns></returns>
        public async Task<bool> DeleteCustomer(string profileId)
        {
            var req = new deleteCustomerProfileRequest {customerProfileId = profileId};

            var response = (deleteCustomerProfileResponse) await _gateway.Send(req);

            return true;
        }

        /// <summary>
        ///     Deletes a payment profile for a customer from the AuthNET servers.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <returns></returns>
        public async Task<bool> DeletePaymentProfile(string profileId, string paymentProfileId)
        {
            var req = new deleteCustomerPaymentProfileRequest
            {
                customerPaymentProfileId = paymentProfileId,
                customerProfileId = profileId
            };

            var response = (deleteCustomerPaymentProfileResponse) await _gateway.Send(req);

            return true;
        }

        /// <summary>
        ///     Deletes a shipping address for a given user from the AuthNET servers.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="shippingAddressId">The shipping address ID.</param>
        /// <returns></returns>
        public async Task<bool> DeleteShippingAddress(string profileId, string shippingAddressId)
        {
            var req = new deleteCustomerShippingAddressRequest
            {
                customerAddressId = shippingAddressId,
                customerProfileId = profileId
            };

            var response = (deleteCustomerShippingAddressResponse) await _gateway.Send(req);

            return true;
        }

        /// <summary>
        ///     Returns all Customer IDs stored at Authorize.NET
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetCustomerIDs()
        {
            var req = new getCustomerProfileIdsRequest();

            var response = (getCustomerProfileIdsResponse) await _gateway.Send(req);

            return response.ids;
        }

        /// <summary>
        ///     Gets a shipping address for a customer.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="shippingAddressId">The shipping address ID.</param>
        /// <returns></returns>
        public async Task<Address> GetShippingAddress(string profileId, string shippingAddressId)
        {
            var req = new getCustomerShippingAddressRequest
            {
                customerAddressId = shippingAddressId,
                customerProfileId = profileId
            };

            var response = (getCustomerShippingAddressResponse) await _gateway.Send(req);

            return new Address(response.address);
        }

        /// <summary>
        ///     Updates a customer's record.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        public async Task<bool> UpdateCustomer(Customer customer)
        {
            var req = new updateCustomerProfileRequest
            {
                profile = new customerProfileExType
                {
                    customerProfileId = customer.ProfileID,
                    description = customer.Description,
                    email = customer.Email,
                    merchantCustomerId = customer.ID
                }
            };

            var response = (updateCustomerProfileResponse) await _gateway.Send(req);

            return true;
        }

        /// <summary>
        ///     Updates a payment profile for a user.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="profile">The profile.</param>
        /// <returns></returns>
        public async Task<bool> UpdatePaymentProfile(string profileId, PaymentProfile profile)
        {
            var req = new updateCustomerPaymentProfileRequest();

            req.customerProfileId = profileId;
            req.paymentProfile = profile.ToAPI();

            if (profile.BillingAddress != null)
                req.paymentProfile.billTo = profile.BillingAddress.ToAPIType();

            var response = (updateCustomerPaymentProfileResponse) await _gateway.Send(req);

            return true;
        }


        /// <summary>
        ///     Updates a shipping address for a user.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public async Task<bool> UpdateShippingAddress(string profileId, Address address)
        {
            var req = new updateCustomerShippingAddressRequest();

            req.customerProfileId = profileId;
            req.address = address.ToAPIExType();
            var response = (updateCustomerShippingAddressResponse) await _gateway.Send(req);

            return true;
        }

        /// <summary>
        ///     Overload method ommitting shippingAddressID.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public Task<string> ValidateProfile(string profileId, string paymentProfileId, ValidationMode mode)
        {
            return ValidateProfile(profileId, paymentProfileId, null, mode);
        }

        /// <summary>
        ///     This function validates the information on a profile - making sure what you have stored at AuthNET is valid. You
        ///     can
        ///     do this in two ways: in TestMode it will just run a validation to be sure all required fields are present and
        ///     valid. If
        ///     you specify "live" - a live authorization request will be performed.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="shippingAddressId">The shipping address ID.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public async Task<string> ValidateProfile(string profileId, string paymentProfileId, string shippingAddressId, ValidationMode mode)
        {
            var req = new validateCustomerPaymentProfileRequest
            {
                customerProfileId = profileId,
                customerPaymentProfileId = paymentProfileId
            };


            if (!string.IsNullOrEmpty(shippingAddressId))
            {
                req.customerShippingAddressId = shippingAddressId;
            }
            req.validationMode = Customer.ToValidationMode(mode);

            var response = (validateCustomerPaymentProfileResponse) await _gateway.Send(req);
            return response.directResponse;
        }

        public async Task<Customer> CreateCustomer(string email, string description, string merchantCustomerId)
        {
            //use the XSD class to create the profile
            var newCustomer = new customerProfileType
            {
                description = description,
                email = email,
                merchantCustomerId = merchantCustomerId
            };

            var req = new createCustomerProfileRequest {profile = newCustomer};


            //serialize and send
            var response = (createCustomerProfileResponse) await _gateway.Send(req);

            //set the profile ID
            return new Customer
            {
                Email = email,
                Description = description,
                ProfileID = response.customerProfileId,
                ID = merchantCustomerId != "" ? merchantCustomerId : "MerchantCustomerID"
            };
        }

        /// <summary>
        ///     Adds a credit card profile to the user and returns the profile ID
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonth">The expiration month.</param>
        /// <param name="expirationYear">The expiration year.</param>
        /// <returns></returns>
        public Task<string> AddCreditCard(string profileId, string cardNumber, int expirationMonth, int expirationYear)
        {
            return AddCreditCard(profileId, cardNumber, expirationMonth, expirationYear, null, null);
        }

        /// <summary>
        ///     Captures the specified transaction.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="shippingProfileId">The id of the shipping information to use for the transaction.</param>
        /// <param name="transId">The transaction id to mark to capture (settle).</param>
        /// <param name="amount">The decimal amount to capture.</param>
        /// <returns></returns>
        public async Task<IGatewayResponse> PriorAuthCapture(string profileId, string paymentProfileId, string shippingProfileId,
            string transId, decimal amount)
        {
            var req = new createCustomerProfileTransactionRequest();

            var trans = new profileTransPriorAuthCaptureType();
            if (!string.IsNullOrEmpty(profileId)) trans.customerProfileId = profileId;
            if (!string.IsNullOrEmpty(paymentProfileId)) trans.customerPaymentProfileId = paymentProfileId;
            trans.transId = transId; //required
            trans.amount = amount; // required.
            if (!string.IsNullOrEmpty(shippingProfileId)) trans.customerShippingAddressId = shippingProfileId;

            req.transaction = new profileTransactionType();
            req.transaction.Item = trans;

            var response = (createCustomerProfileTransactionResponse) await _gateway.Send(req);
            return new GatewayResponse(response.directResponse.Split(','));
        }

        /// <summary>
        ///     Captures the specified transaction.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile id.</param>
        /// <param name="transId">The transaction id to mark to capture (settle).</param>
        /// <param name="amount">The decimal amount to capture.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> PriorAuthCapture(string profileId, string paymentProfileId, string transId, decimal amount)
        {
            return PriorAuthCapture(profileId, paymentProfileId, null, transId, amount);
        }

        /// <summary>
        ///     Captures the specified transaction.
        /// </summary>
        /// <param name="transId">The transaction id to mark to capture (settle).</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public Task<IGatewayResponse> PriorAuthCapture(string transId, decimal amount)
        {
            return PriorAuthCapture(string.Empty, string.Empty, transId, amount);
        }

        /// <summary>
        ///     Overload method ommitting shippingAddressID &amp; ValidationMode enum so will use the value passed into the
        ///     constructor.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <returns></returns>
        public Task<string> ValidateProfile(string profileId, string paymentProfileId)
        {
            return ValidateProfile(profileId, paymentProfileId, null, ValidationMode.None);
        }

        /// <summary>
        ///     Overload method ommitting ValidationMode enum so will use the value passed into the constructor.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="paymentProfileId">The payment profile ID.</param>
        /// <param name="shippingAddressId">The shipping address ID.</param>
        /// <returns></returns>
        public Task<string> ValidateProfile(string profileId, string paymentProfileId, string shippingAddressId)
        {
            return ValidateProfile(profileId, paymentProfileId, shippingAddressId, ValidationMode.None);
        }
    }
}