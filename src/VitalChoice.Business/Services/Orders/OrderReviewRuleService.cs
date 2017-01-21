using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Workflow.Core;
using Microsoft.Extensions.Logging;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Business.Mailings;
using VitalChoice.Data.UOW;
using VitalChoice.Infrastructure.Domain.Entities.Checkout;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderReviewRuleService : ExtendedEcommerceDynamicService<OrderReviewRuleDynamic, OrderReviewRule,
        OrderReviewRuleOptionType, OrderReviewRuleOptionValue>,
        IOrderReviewRuleService
    {
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly OrderReviewRuleMapper _mapper;
        private readonly ReferenceData _referenceData;
        private readonly AppSettings _appSettings;
        private readonly ILoggerFactory _loggerFactory;

        public OrderReviewRuleService(
            IEcommerceRepositoryAsync<OrderReviewRule> ruleRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderReviewRuleMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderReviewRuleOptionValue> ruleValueRepositoryAsync,
            ILoggerFactory loggerProvider,
            DynamicExtensionsRewriter queryVisitor,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IDynamicEntityOrderingExtension<OrderReviewRule> orderingExtension,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ReferenceData referenceData, 
            AppSettings appSettings)
            : base(
                mapper, ruleRepository, ruleValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension
                )
        {
            _mapper = mapper;
            _adminProfileRepository = adminProfileRepository;
            _referenceData = referenceData;
            _appSettings = appSettings;
            _loggerFactory = loggerProvider;
        }

        public async Task<PagedList<OrderReviewRuleDynamic>> GetShortOrderReviewRulesAsync(FilterBase filter)
        {
            var conditions = new OrderReviewRuleQuery().NotDeleted().WithName(filter.SearchText);
            var query = ObjectRepository.Query(conditions);

            Func<IQueryable<OrderReviewRule>, IOrderedQueryable<OrderReviewRule>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;

            PagedList<OrderReviewRule> result;
            if (filter.Paging != null)
            {
                result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await query.OrderBy(sortable).SelectAsync(false);
                result = new PagedList<OrderReviewRule>(items, items.Count);
            }

            foreach (var item in result.Items)
            {
                item.OptionValues = new List<OrderReviewRuleOptionValue>();
                item.OptionTypes = new List<OrderReviewRuleOptionType>();
            }
            PagedList<OrderReviewRuleDynamic> toReturn = new PagedList<OrderReviewRuleDynamic>(result.Items.Select
                (p => _mapper.FromEntity(p)).ToList(), result.Count);
            if (toReturn.Items.Count > 0)
            {
                var ids = result.Items.Where(p => p.IdAddedBy.HasValue).Select(p => p.IdAddedBy.Value).ToList();
                ids.AddRange(result.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value));
                ids = ids.Distinct().ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdAddedBy == profile.Id)
                        {
                            item.Data.AddedByAgentId = profile.AgentId;
                        }
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.Data.EditedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        public Task<List<OrderReviewRuleDynamic>> GetAllRules() => SelectAsync(rule => true, withDefaults: true);
    }
}