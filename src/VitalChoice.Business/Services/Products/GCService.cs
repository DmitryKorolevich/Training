using System;
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

        public GCService(IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository,
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService,
            OrderAddressMapper orderAddressMapper,
            ILoggerProviderExtended loggerProvider)
        {
            this.giftCertificateRepository = giftCertificateRepository;
            this.userManager = userManager;
            this.notificationService = notificationService;
            this.orderAddressMapper = orderAddressMapper;
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
            //Get all GCs from cache
            var data = await giftCertificateRepository.Query(p=>p.StatusCode!=RecordStatusCode.Deleted).
                Include(p=>p.Order).ThenInclude(p=>p.ShippingAddress).ThenInclude(p=>p.OptionValues).
                Include(p=>p.Order).ThenInclude(p=>p.PaymentMethod).ThenInclude(p=>p.BillingAddress).ThenInclude(p=>p.OptionValues).
                SelectAsync(false);

            var query = data.Select(p => new GCWithOrderListItemModel(p, orderAddressMapper.OptionTypes));
            if(filter.From.HasValue)
            {
                query = query.Where(p => p.Created >= filter.From.Value);
            }
            if (filter.To.HasValue)
            {
                query = query.Where(p => p.Created <= filter.To.Value);
            }
            if (filter.Type.HasValue)
            {
                query = query.Where(p => p.GCType==filter.Type.Value);
            }
            if (filter.StatusCode.HasValue)
            {
                query = query.Where(p => p.StatusCode == filter.StatusCode.Value);
            }
            if (filter.ShippingAddress!=null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
            {
                query = query.Where(p => !String.IsNullOrEmpty(p.ShippingLastName) && p.ShippingLastName.IndexOf(filter.ShippingAddress.LastName, StringComparison.OrdinalIgnoreCase)>=0);
            }
            if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
            {
                query = query.Where(p => !String.IsNullOrEmpty(p.BillingLastName) && p.BillingLastName.IndexOf(filter.BillingAddress.LastName, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            Func<IQueryable<GCWithOrderListItemModel>, IOrderedQueryable<GCWithOrderListItemModel>> sortable = x => x.OrderByDescending(y => y.Created);
            var sortOrder = filter.Sorting.SortOrder;
            if(String.IsNullOrEmpty(filter.Sorting.Path))
            {
                filter.Sorting.Path = GiftCertificatesWithOrderSortPath.Created;
            }
            switch (filter.Sorting.Path)
            {
                case GiftCertificatesWithOrderSortPath.Code:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.Code)
                                : query.OrderByDescending(y => y.Code);
                    break;
                case GiftCertificatesWithOrderSortPath.Created:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.Created)
                                : query.OrderByDescending(y => y.Created);
                    break;
                case GiftCertificatesWithOrderSortPath.BillingLastName:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.BillingLastName)
                                : query.OrderByDescending(y => y.BillingLastName);
                    break;
                case GiftCertificatesWithOrderSortPath.ShippingLastName:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.ShippingLastName)
                                : query.OrderByDescending(y => y.ShippingLastName);
                    break;
                case GiftCertificatesWithOrderSortPath.Type:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.GCType)
                                : query.OrderByDescending(y => y.GCType);
                    break;
                case GiftCertificatesWithOrderSortPath.Status:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.StatusCode)
                                : query.OrderByDescending(y => y.StatusCode);
                    break;
                case GiftCertificatesWithOrderSortPath.Balance:
                    query = sortOrder == FilterSortOrder.Asc
                                ? query.OrderBy(y => y.Balance)
                                : query.OrderByDescending(y => y.Balance);
                    break;
            }

            var items = query.ToList();
            GCStatisticModel toReturn = new GCStatisticModel();
            toReturn.Count = items.Count;
            toReturn.Total = items.Where(p=>p.StatusCode==RecordStatusCode.Active).Sum(p => p.Balance);
            if (filter.Paging != null)
            {
                toReturn.Items = items.Skip((filter.Paging.PageIndex-1)* filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount).ToList();
            }
            else
            {
                toReturn.Items = items;
            }
            foreach(var item in items)
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