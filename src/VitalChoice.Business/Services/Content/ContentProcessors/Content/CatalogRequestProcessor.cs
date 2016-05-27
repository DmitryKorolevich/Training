﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Models.Help;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.ObjectMapping.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.CatalogRequests;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Content
{
    public class CatalogRequestProcessor : ContentProcessor<CatalogRequestAddressModel, object, ContentPage>
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDynamicMapper<AddressDynamic, CatalogRequestAddress> _catalogRequestAddressMapper;
        private readonly ExtendedUserManager _userManager;

        public CatalogRequestProcessor(IObjectMapper<object> mapper,
            ICustomerService customerService,
            IAuthorizationService authorizationService,
            IDynamicMapper<AddressDynamic, CatalogRequestAddress> catalogRequestAddressMapper, ExtendedUserManager userManager) : base(mapper)
        {
            _customerService = customerService;
            _authorizationService = authorizationService;
            _catalogRequestAddressMapper = catalogRequestAddressMapper;
            _userManager = userManager;
        }

        protected override async Task<CatalogRequestAddressModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null || viewContext.User==null)
            {
                throw new ApiException("Invalid data");
            }

            CatalogRequestAddressModel toReturn = null;
            var claimUser = viewContext.User;
            var result = await _authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
            if (result)
            {
                if (!claimUser.HasClaim(x => x.Type == IdentityConstants.NotConfirmedClaimType) &&
                    claimUser.HasClaim(x => x.Type == IdentityConstants.CustomerRoleType))
                {
                    var internalId = Convert.ToInt32(_userManager.GetUserId(viewContext.User));
                    var customer = await _customerService.SelectAsync(internalId);
                    if (customer?.ProfileAddress != null)
                    {
                        toReturn = await _catalogRequestAddressMapper.ToModelAsync<CatalogRequestAddressModel>(customer.ProfileAddress);
                    }
                }
            }

            return toReturn;
        }

        public override string ResultName => "CatalogRequest";
    }
}