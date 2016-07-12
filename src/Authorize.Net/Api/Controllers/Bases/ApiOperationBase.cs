using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Util;
using Microsoft.Extensions.Logging;

namespace Authorize.Net.Api.Controllers.Bases
{
    /**
     * @author ramittal
     *
     */
#pragma warning disable 1591
    public abstract class ApiOperationBase<TQ, TS> : IApiOperation<TQ, TS>
        where TQ : ANetApiRequest
        where TS : ANetApiResponse
    {
        private const string NullEnvironmentErrorMessage =
            "Environment not set. Set environment using setter or use overloaded method to pass appropriate environment";

        private readonly Type _responseClass;
        private TQ _apiRequest;
        private TS _apiResponse;

        private ANetApiResponse _errorResponse;
        protected messageTypeEnum ResultCode = messageTypeEnum.Ok;

        protected List<string> Results;

        protected ApiOperationBase(TQ apiRequest)
        {
            if (null == apiRequest)
            {
                throw new ArgumentNullException(nameof(apiRequest), "Input request cannot be null");
            }
            if (null != GetApiResponse())
            {
                throw new InvalidOperationException("Response should be null");
            }

            _responseClass = typeof (TS);
            SetApiRequest(apiRequest);
        }

        public Environment RunEnvironment { get; set; }
        public merchantAuthenticationType MerchantAuthentication { get; set; }

        public TS GetApiResponse()
        {
            return _apiResponse;
        }

        public ANetApiResponse GetErrorResponse()
        {
            return _errorResponse;
        }

        public async Task<TS> ExecuteWithApiResponse(Environment environment = null)
        {
            await Execute(environment);
            return GetApiResponse();
        }

        public async Task Execute(Environment environment = null)
        {
            Validate();
            BeforeExecute();

            if (null == environment)
            {
                environment = RunEnvironment;
            }
            if (null == environment) throw new ArgumentException(NullEnvironmentErrorMessage);

            var httpApiResponse = await HttpUtility.PostData<TQ, TS>(environment, GetApiRequest());

            if (null != httpApiResponse)
            {
                if (httpApiResponse.GetType() == _responseClass)
                {
                    var response = (TS) httpApiResponse;
                    SetApiResponse(response);
                }
                else if (httpApiResponse.GetType() == typeof (ErrorResponse))
                {
                    SetErrorResponse(httpApiResponse);
                }
                else
                {
                    SetErrorResponse(httpApiResponse);
                }
                SetResultStatus();
            }
            AfterExecute();
        }

        public messageTypeEnum GetResultCode()
        {
            return ResultCode;
        }

        public List<string> GetResults()
        {
            return Results;
        }

        protected TQ GetApiRequest()
        {
            return _apiRequest;
        }

        protected void SetApiRequest(TQ apiRequest)
        {
            _apiRequest = apiRequest;
        }

        private void SetApiResponse(TS apiResponse)
        {
            _apiResponse = apiResponse;
        }

        private void SetErrorResponse(ANetApiResponse errorResponse)
        {
            _errorResponse = errorResponse;
        }

        private void SetResultStatus()
        {
            Results = new List<string>();
            var messageTypes = GetResultMessage();

            if (null != messageTypes)
            {
                ResultCode = messageTypes.resultCode;
            }

            if (null != messageTypes)
            {
                foreach (var amessage in messageTypes.message)
                {
                    Results.Add(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", amessage.code, amessage.text));
                }
            }
        }

        private messagesType GetResultMessage()
        {
            messagesType messageTypes = null;

            if (null != GetErrorResponse())
            {
                messageTypes = GetErrorResponse().messages;
            }
            else if (null != GetApiResponse())
            {
                messageTypes = GetApiResponse().messages;
            }

            return messageTypes;
        }

        protected virtual void BeforeExecute()
        {
        }

        protected virtual void AfterExecute()
        {
        }

        protected abstract void ValidateRequest();

        private void Validate()
        {
            ANetApiRequest request = GetApiRequest();

            //validate not nulls
            ValidateAndSetMerchantAuthentication();

            //validate nulls
            var merchantAuthenticationType = request.merchantAuthentication;
            //if ( null != ) throw new IllegalArgumentException(" needs to be null");

            //NOTTODO, BG
            /*
		    if ( null != merchantAuthenticationType.Item.GetType().   sessionToken) throw new IllegalArgumentException("SessionToken needs to be null");
		    if ( null != merchantAuthenticationType.getPassword()) throw new IllegalArgumentException("Password needs to be null");
		    if ( null != merchantAuthenticationType.getMobileDeviceId()) throw new IllegalArgumentException("MobileDeviceId needs to be null");
             
	    
	        var impersonationAuthenticationType = merchantAuthenticationType.impersonationAuthentication;
		    if ( null != impersonationAuthenticationType) throw new IllegalArgumentException("ImpersonationAuthenticationType needs to be null");
            */
            //	    impersonationAuthenticationType.setPartnerLoginId(CnpApiLoginIdKey);
            //	    impersonationAuthenticationType.setPartnerTransactionKey(CnpTransactionKey);
            //	    merchantAuthenticationType.setImpersonationAuthentication(impersonationAuthenticationType);

            ValidateRequest();
        }

        private void ValidateAndSetMerchantAuthentication()
        {
            ANetApiRequest request = GetApiRequest();

            if (null == request.merchantAuthentication)
            {
                if (null != MerchantAuthentication)
                {
                    request.merchantAuthentication = MerchantAuthentication;
                }
                else
                {
                    throw new ArgumentException("MerchantAuthentication cannot be null");
                }
            }
        }
    }
#pragma warning restore 1591
}