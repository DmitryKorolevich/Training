﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Data.Helpers;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services.Bronto;
using Microsoft.EntityFrameworkCore;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.Services.Products
{
    public class GCService : IGcService
    {
        private const int GC_SYMBOLS_COUNT = 12;

        private static readonly List<char> symbols = new List<char>() { '0','1','2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'T', 'U', 'V', 'W', 'X','Y','Z' };

        private readonly IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly INotificationService notificationService;
        private readonly OrderAddressMapper orderAddressMapper;
        private readonly ILogger logger;
        private readonly DynamicExtensionsRewriter queryVisitor;

        public GCService(IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository,
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService,
            OrderAddressMapper orderAddressMapper,
            ILoggerProviderExtended loggerProvider,
            DynamicExtensionsRewriter queryVisitor)
        {
            this.giftCertificateRepository = giftCertificateRepository;
            this.userManager = userManager;
            this.notificationService = notificationService;
            this.orderAddressMapper = orderAddressMapper;
            this.queryVisitor = queryVisitor;
            logger = loggerProvider.CreateLogger<GCService>();
        }

        public async Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter)
        {
            var conditions = new GcQuery().NotDeleted().WithType(filter.Type).WidthStatus(filter.StatusCode).WithCode(filter.Code).WithEmail(filter.Email).WithName(filter.Name);
            var query = giftCertificateRepository.Query(conditions);

            Func<IQueryable<GiftCertificate>, IOrderedQueryable<GiftCertificate>> sortable = x => x.OrderByDescending(y => y.Created);
			var sortOrder = filter.Sorting.SortOrder;
	        switch (filter.Sorting.Path)
	        {
		        case GiftCertificateSortPath.Recipient:
			        sortable =
				        (x) =>
					        sortOrder == FilterSortOrder.Asc
						        ? x.OrderBy(y => y.FirstName).ThenBy(y => y.LastName)
						        : x.OrderByDescending(y => y.FirstName).ThenByDescending(y => y.LastName);
			        break;
		        case GiftCertificateSortPath.Available:
			        sortable =
				        (x) =>
					        sortOrder == FilterSortOrder.Asc
						        ? x.OrderBy(y => y.Balance)
						        : x.OrderByDescending(y => y.Balance);
			        break;
		        case GiftCertificateSortPath.Status:
			        sortable =
				        (x) =>
					        sortOrder == FilterSortOrder.Asc
						        ? x.OrderBy(y => y.StatusCode)
						        : x.OrderByDescending(y => y.StatusCode);
			        break;
		        case GiftCertificateSortPath.Created:
					sortable =
						(x) =>
							sortOrder == FilterSortOrder.Asc
								? x.OrderBy(y => y.Created)
								: x.OrderByDescending(y => y.Created);
			        break;
	        }

	        PagedList<GiftCertificate> toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var userIds = toReturn.Items.Select(pp => pp.UserId).Where(u => u.HasValue).Select(u => u.Value).Distinct().ToList();
            var users =
                await
                    userManager.Users.AsNoTracking().Where(p => userIds.Contains(p.Id))
                        .Include(x => x.Profile)
                        .ToListAsync();
            foreach (var item in toReturn.Items)
            {
                foreach (var user in users)
                {
                    if (item.UserId == user.Id)
                    {
                        item.AgentId = user.Profile?.AgentId;
                        break;
                    }
                }
            }

            return toReturn;
        }

        public async Task<GCStatisticModel> GetGiftCertificatesWithOrderInfoAsync(GCFilter filter)
        {
            GcQuery conditions = new GcQuery().NotDeleted().WithFrom(filter.From).WithTo(filter.To).WithType(filter.Type).
                WidthStatus(filter.StatusCode);
            if (filter.ShippingAddress != null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
            {
                conditions = conditions.WithShippingAddress(filter.ShippingAddress);
            }
            if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
            {
                conditions = conditions.WithBillingAddress(filter.BillingAddress);
            }
            var q = (Expression<Func<GiftCertificate, bool>>)queryVisitor.Visit(conditions.Query());

            Func<IQueryable<GiftCertificate>, IOrderedQueryable<GiftCertificate>> sortable = x => x.OrderByDescending(y => y.Created);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case GiftCertificatesWithOrderSortPath.Code:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Code)
                                : x.OrderByDescending(y => y.Code);
                    break;
                case GiftCertificatesWithOrderSortPath.Created:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Created)
                                : x.OrderByDescending(y => y.Created);
                    break;
                case GiftCertificatesWithOrderSortPath.Type:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.GCType)
                                : x.OrderByDescending(y => y.GCType);
                    break;
                case GiftCertificatesWithOrderSortPath.Status:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
                case GiftCertificatesWithOrderSortPath.Balance:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Balance)
                                : x.OrderByDescending(y => y.Balance);
                    break;
            }

            var query = giftCertificateRepository.Query(q).
                Include(p => p.Order).ThenInclude(p => p.PaymentMethod).ThenInclude(p => p.BillingAddress).ThenInclude(p => p.OptionValues).
                Include(p=>p.Order).ThenInclude(p=>p.ShippingAddress).ThenInclude(p=>p.OptionValues).OrderBy(sortable);
            
            GCStatisticModel toReturn = new GCStatisticModel();
            if (filter.Paging != null)
            {
                var data = await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
                toReturn.Items = data.Items.Select(p => new GCWithOrderListItemModel(p, orderAddressMapper.OptionTypes)).ToList();
                toReturn.Count = data.Count;
                
                var balanceConditions  =  new GcQuery().NotDeleted().WithFrom(filter.From).WithTo(filter.To).WithType(filter.Type).
                    WidthStatus(RecordStatusCode.Active);
                if (filter.ShippingAddress != null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
                {
                    balanceConditions = conditions.WithShippingAddress(filter.ShippingAddress);
                }
                if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
                {
                    balanceConditions = conditions.WithBillingAddress(filter.BillingAddress);
                }
                q = (Expression<Func<GiftCertificate, bool>>)queryVisitor.Visit(balanceConditions.Query());
                toReturn.Total = await giftCertificateRepository.Query(q).SelectSumAsync(p => p.Balance);
            }
            else
            {
                var data = await query.SelectAsync(false);
                toReturn.Items = data.Select(p=> new GCWithOrderListItemModel(p, orderAddressMapper.OptionTypes)).ToList();
                toReturn.Count = toReturn.Items.Count;
                toReturn.Total = toReturn.Items.Where(p => p.StatusCode == RecordStatusCode.Active).Sum(p => p.Balance);
            }
            foreach(var item in toReturn.Items)
            {
                item.GCTypeName = LookupHelper.GetShortGCTypeName(item.GCType);
                item.StatusCodeName = LookupHelper.GetRecordStatus(item.StatusCode);
            }

            return toReturn;
        }

        public Task<GiftCertificate> GetGiftCertificateAsync(int id)
        {
            var conditions = new GcQuery().WithId(id).NotDeleted();
            var query = giftCertificateRepository.Query(conditions);

            return query.SelectFirstOrDefaultAsync(false);
        }

        public Task<GiftCertificate> GetGiftCertificateAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Task.FromResult<GiftCertificate>(null);
            var query = giftCertificateRepository.Query(new GcQuery().WithEqualCode(code).NotDeleted());

            return query.SelectFirstOrDefaultAsync(false);
        }

        public async Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model)
        {
            var conditions = new GcQuery().WithId(model.Id).NotDeleted();
            var query = giftCertificateRepository.Query(conditions);

            GiftCertificate dbItem = await query.SelectFirstOrDefaultAsync(true);

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                dbItem.FirstName = model.FirstName;
                dbItem.LastName = model.LastName;
                dbItem.Email = model.Email;
                dbItem.Balance = model.Balance;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }

                await giftCertificateRepository.UpdateAsync(dbItem);
            }

            return dbItem;
        }

        public async Task<ICollection<GiftCertificate>> AddManualGiftCertificatesAsync(int quantity, GiftCertificate model)
        {
            List<GiftCertificate> items = new List<GiftCertificate>();
            DateTime now = DateTime.Now;
            for(int i=0;i<quantity;i++)
            {
                GiftCertificate item = (GiftCertificate)model.Clone();
                item.Id = 0;
                item.Created = now;
                item.GCType = GCType.ManualGC;
                item.StatusCode = RecordStatusCode.Active;
                item.Code = await GenerateGCCode();
                items.Add(item);
            }

            await giftCertificateRepository.InsertRangeAsync(items);

            return items;
        }

        public async Task<bool> SendGiftCertificateEmailAsync(BasicEmail model)
        {
            await notificationService.SendBasicEmailAsync(model);
            return true;
        }

        public async Task<bool> DeleteGiftCertificateAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await giftCertificateRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await giftCertificateRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        public async Task<List<GiftCertificate>> GetGiftCertificatesAsync(Expression<Func<GiftCertificate, bool>> expression)
        {
            return await giftCertificateRepository.Query(expression).SelectAsync(false);
        }

        public async Task<string> GenerateGCCode()
        {
            string toReturn = null;
            bool generated = false;
            while (!generated)
            {
                var attempt = string.Empty;
                Guid guid = Guid.NewGuid();
                var bytes = guid.ToByteArray();
                for(int i= bytes.Length-1; i >= bytes.Length- GC_SYMBOLS_COUNT; i--)
                {
                    attempt += symbols[bytes[i] % symbols.Count];
                }
                GiftCertificate dbItem = await giftCertificateRepository.Query(p => p.Code == attempt).SelectFirstOrDefaultAsync(false);
                if (dbItem == null)
                {
                    generated = true;
                    toReturn = attempt;
                }
            }
            return toReturn;
        }
    }
}