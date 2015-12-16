﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Payments;

namespace VitalChoice.Business.Services.Payment
{
	public class PaymentMethodService : IPaymentMethodService
	{
		private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepository;
		private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
		private readonly EcommerceContext _context;
		private readonly IEcommerceRepositoryAsync<PaymentMethodToCustomerType> _paymentMethodToCustomerTypeRepository;
		private readonly IHttpContextAccessor _contextAccessor;
	    private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _mapper;
	    private readonly IOptions<AppOptions> _options;
	    private readonly ICountryNameCodeResolver _countryNameCode;
	    private readonly ILogger _logger;

	    public PaymentMethodService(IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository,
	        IHttpContextAccessor contextAccessor, IRepositoryAsync<AdminProfile> adminProfileRepository,
	        EcommerceContext context,
	        IEcommerceRepositoryAsync<PaymentMethodToCustomerType> paymentMethodToCustomerTypeRepository,
	        ILoggerProviderExtended loggerProvider, IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> mapper, IOptions<AppOptions> options, ICountryNameCodeResolver countryNameCode)
	    {
	        _paymentMethodRepository = paymentMethodRepository;
	        _contextAccessor = contextAccessor;
	        _adminProfileRepository = adminProfileRepository;
	        _context = context;
	        _paymentMethodToCustomerTypeRepository = paymentMethodToCustomerTypeRepository;
	        _mapper = mapper;
	        _options = options;
	        _countryNameCode = countryNameCode;
	        _logger = loggerProvider.CreateLoggerDefault();
	    }

	    public async Task<IList<ExtendedPaymentMethod>> GetApprovedPaymentMethodsAsync()
		{
			var condition = new PaymentMethodQuery().NotDeleted();

			var query = _paymentMethodRepository.Query(condition).OrderBy(x => x.OrderBy(y => y.Name));

			var paymentMethods = await query.Include(x => x.CustomerTypes).SelectAsync(false);

			var adminProfileCondition =
				new AdminProfileQuery().IdInRange(
					paymentMethods.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

			var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

			var result = paymentMethods.Select(paymentMethod => new ExtendedPaymentMethod
			{
				AdminProfile = adminProfiles.SingleOrDefault(x => x.Id == paymentMethod.IdEditedBy),
				IdEditedBy = paymentMethod.IdEditedBy,
				CustomerTypes = paymentMethod.CustomerTypes,
				DateCreated = paymentMethod.DateCreated,
				DateEdited = paymentMethod.DateEdited,
				EditedBy = paymentMethod.EditedBy,
				Id = paymentMethod.Id,
				Name = paymentMethod.Name,
				Order = paymentMethod.Order,
				StatusCode = paymentMethod.StatusCode
			}).ToList();

			return result;
		}

		public async Task SetStateAsync(IList<PaymentMethodsAvailability> paymentMethodsAvailability)
		{
			var currentUserId = Convert.ToInt32(_contextAccessor.HttpContext.User.GetUserId());

			var paymentMethods =
				await
					_paymentMethodRepository.Query(new PaymentMethodQuery().NotDeleted())
						.Include(x => x.CustomerTypes)
						.SelectAsync(false);

			using (var transaction = new TransactionAccessor(_context).BeginTransaction())
			{
				try
				{
					foreach (var paymentMethod in paymentMethods)
					{
						var needToUpdate = false;

						var assignment = paymentMethodsAvailability.FirstOrDefault(x => x.Id == paymentMethod.Id);
						if (assignment?.CustomerTypes != null && assignment.CustomerTypes.Any())
						{
							if (!paymentMethod.CustomerTypes.Any() || paymentMethod.CustomerTypes.Count != assignment.CustomerTypes.Count || !paymentMethod.CustomerTypes.All(x => assignment.CustomerTypes.Contains(x.IdCustomerType)))
							{
								await _paymentMethodToCustomerTypeRepository.DeleteAllAsync(paymentMethod.CustomerTypes);
                                paymentMethod.CustomerTypes.Clear();
								foreach (var customerType in assignment.CustomerTypes)
								{
									paymentMethod.CustomerTypes.Add(new PaymentMethodToCustomerType()
									{
										IdCustomerType = customerType,
										IdPaymentMethod = paymentMethod.Id,
									});
								}
								if (assignment.CustomerTypes.Any())
								{
									await _paymentMethodToCustomerTypeRepository.InsertRangeAsync(paymentMethod.CustomerTypes);
								}
								needToUpdate = true;
							}
						}
						else
						{
							if (paymentMethod.CustomerTypes != null && paymentMethod.CustomerTypes.Any())
							{
								await _paymentMethodToCustomerTypeRepository.DeleteAllAsync(paymentMethod.CustomerTypes);
                                paymentMethod.CustomerTypes.Clear();
                                needToUpdate = true;
							}
						}

						if (needToUpdate)
						{
							paymentMethod.DateEdited = DateTime.Now;
							paymentMethod.IdEditedBy = currentUserId;
							await _paymentMethodRepository.UpdateAsync(paymentMethod);
						}
					}

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public async Task<PaymentMethod> GetStorefrontDefaultPaymentMenthod()
		{
			var condition = new PaymentMethodQuery().NotDeleted().CreditCard();

			var creditCard = await _paymentMethodRepository.Query(condition).SelectFirstOrDefaultAsync(false);

			return creditCard;
		}

        public async Task<List<MessageInfo>> AuthorizeCreditCard(PaymentMethodDynamic paymentMethod)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            if (_mapper.IsObjectSecured(paymentMethod))
                return errors;

            if (!ValidateCreditCard(paymentMethod))
            {
                errors.Add(new MessageInfo
                {
                    Field = "CardNumber",
                    Message = "Invalid credit card number. Please try to change card type or fix the number"
                });
                return errors;
            }

            if (_options.Value.AuthorizeNet.TestEnv || paymentMethod.IdObjectType != (int)PaymentMethodType.CreditCard)
                return errors;

            var creditCard = new creditCardType
            {
                cardNumber = paymentMethod.Data.CardNumber,
                expirationDate = ((DateTime)paymentMethod.Data.ExpDate).ToString("MMyy"),
                isPaymentToken = false
            };

            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authOnlyTransaction.ToString(), // authorize only
                amount = 1,
                payment = paymentType,
                order = new orderType
                {
                    invoiceNumber = $"{paymentMethod.Id}_{DateTime.Now.ToString("hmmss")}"
                },
                billTo = new customerAddressType
                {
                    address = paymentMethod.Address.Data.Address1,
                    city = paymentMethod.Address.Data.City,
                    company = paymentMethod.Address.SafeData.Company,
                    country = _countryNameCode.GetCountryCode(paymentMethod.Address),
                    email = paymentMethod.Address.SafeData.Email,
                    faxNumber = paymentMethod.Address.SafeData.Fax,
                    firstName = paymentMethod.Address.Data.FirstName,
                    lastName = paymentMethod.Address.Data.LastName,
                    phoneNumber = paymentMethod.Address.Data.Phone,
                    state = _countryNameCode.GetRegionOrStateCode(paymentMethod.Address),
                    zip = paymentMethod.Address.Data.Zip
                }
            };

            var request = new createTransactionRequest
            {
                transactionRequest = transactionRequest
            };

            var controller = new createTransactionController(request)
            {
                RunEnvironment = Authorize.Net.Environment.Production,
                MerchantAuthentication = new merchantAuthenticationType()
                {
                    name = _options.Value.AuthorizeNet.ApiLogin,
                    ItemElementName = ItemChoiceType.transactionKey,
                    Item = _options.Value.AuthorizeNet.ApiKey
                }
            };
            await controller.Execute();

            var response = controller.GetApiResponse();
            ParseErrors(errors, response);
            if (response.messages.resultCode == messageTypeEnum.Ok && !errors.Any())
            {
                if (response.transactionResponse != null)
                {
                    var voidTransactionRequest = new transactionRequestType
                    {
                        transactionType = transactionTypeEnum.voidTransaction.ToString(),
                        refTransId = response.transactionResponse.transId
                    };

                    request = new createTransactionRequest
                    {
                        transactionRequest = voidTransactionRequest
                    };

                    controller = new createTransactionController(request)
                    {
                        RunEnvironment = Authorize.Net.Environment.Production,
                        MerchantAuthentication = new merchantAuthenticationType()
                        {
                            name = _options.Value.AuthorizeNet.ApiLogin,
                            ItemElementName = ItemChoiceType.transactionKey,
                            Item = _options.Value.AuthorizeNet.ApiKey
                        }
                    };
                    await controller.Execute();

                    response = controller.GetApiResponse();
                    ParseErrors(errors, response);
                    if (response.messages.resultCode == messageTypeEnum.Ok)
                    {
                        return errors;
                    }
                    return errors;
                }
            }
            return errors;
        }

	    public bool ValidateCreditCard(PaymentMethodDynamic paymentMethod)
	    {
	        if (paymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
	        {
	            string cardNumber = paymentMethod.Data.CardNumber;
	            switch ((CreditCardType) paymentMethod.Data.CardType)
	            {
	                case CreditCardType.MasterCard:
	                    if (!PaymentValidationExpressions.MasterCardRegex.IsMatch(cardNumber))
	                    {
	                        return false;
	                    }
	                    break;
	                case CreditCardType.Visa:
	                    if (!PaymentValidationExpressions.VisaRegex.IsMatch(cardNumber))
	                    {
	                        return false;
	                    }
	                    break;
	                case CreditCardType.AmericanExpress:
	                    if (!PaymentValidationExpressions.AmericanExpressRegex.IsMatch(cardNumber))
	                    {
	                        return false;
	                    }
	                    break;
	                case CreditCardType.Discover:
	                    if (!PaymentValidationExpressions.DiscoverRegex.IsMatch(cardNumber))
	                    {
	                        return false;
	                    }
	                    break;
	                default:
	                    return true;
	            }
	        }
	        return true;
	    }

	    private static void ParseErrors(List<MessageInfo> result, createTransactionResponse response)
	    {
	        result.AddRange(response?.messages?.message?.Where(m => m.code?.ToLower() != "i00001").Select(m => new MessageInfo()
	        {
	            Field = "CardNumber", Message = $"[{m.code}] {m.text}"
	        }) ?? Enumerable.Empty<MessageInfo>());
	        result.AddRange(response?.transactionResponse?.errors?.Select(m => new MessageInfo()
	        {
	            Field = "CardNumber", Message = $"[{m.errorCode}] {m.errorText}"
	        }) ?? Enumerable.Empty<MessageInfo>());
	        if (response?.transactionResponse?.avsResultCode == "N")
	        {
	            result.Add(new MessageInfo
	            {
	                Field = "Zip", Message = "Street address and postal code do not match."
	            });
	            result.Add(new MessageInfo
	            {
	                Field = "Address1", Message = "Street address and postal code do not match."
	            });
	        }
	    }
	}
}

