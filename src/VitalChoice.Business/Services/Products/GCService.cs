using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
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
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.Business.Mailings;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
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
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;

        public GCService(IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository,
            UserManager<ApplicationUser> userManager, 
            INotificationService notificationService,
            OrderAddressMapper orderAddressMapper,
            ILoggerFactory loggerProvider,
            DynamicExtensionsRewriter queryVisitor,
            IObjectLogItemExternalService objectLogItemExternalService)
        {
            this.giftCertificateRepository = giftCertificateRepository;
            this.userManager = userManager;
            this.notificationService = notificationService;
            this.orderAddressMapper = orderAddressMapper;
            this.queryVisitor = queryVisitor;
            _objectLogItemExternalService = objectLogItemExternalService;
            logger = loggerProvider.CreateLogger<GCService>();
        }

        public async Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter)
        {
            var conditions = new GcQuery().NotDeleted().WithType(filter.Type).WidthStatus(filter.StatusCode).WithCode(filter.Code);
            if (!string.IsNullOrWhiteSpace(filter.ExactCode))
            {
                conditions = conditions.WithEqualCode(filter.ExactCode);
            }
            conditions = conditions.WithEmail(filter.Email).WithName(filter.Name);
            conditions = conditions.WithExpirationDateFrom(filter.ExpirationFrom).WithExpirationDateTo(filter.ExpirationTo).
                WithTag(filter.Tag).WithNotZeroBalance(filter.NotZeroBalance);
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
                case GiftCertificateSortPath.ExpirationDate:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.ExpirationDate)
                                : x.OrderByDescending(y => y.ExpirationDate);
                    break;
                case GiftCertificateSortPath.Tag:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Tag)
                                : x.OrderByDescending(y => y.Tag);
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
                WidthStatus(filter.StatusCode).WithNotZeroBalance(filter.NotZeroBalance);
            //if (filter.ShippingAddress != null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
            //{
            //    conditions = conditions.WithShippingAddress(filter.ShippingAddress);
            //}
            //if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
            //{
            //    conditions = conditions.WithBillingAddress(filter.BillingAddress);
            //}
            //var q = (Expression<Func<GiftCertificate, bool>>)queryVisitor.Visit(conditions.Query());

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

            var query = giftCertificateRepository.Query(conditions).
                Include(p => p.Order).ThenInclude(p => p.PaymentMethod).ThenInclude(p => p.BillingAddress).ThenInclude(p => p.OptionValues).
                Include(p => p.Order).ThenInclude(p => p.ShippingAddress).ThenInclude(p => p.OptionValues).OrderBy(sortable);

            var data = await query.SelectAsync(false);
            var billingId =
                orderAddressMapper.OptionTypes
                    .First(t => (t.IdObjectType == null || t.IdObjectType == (int) AddressType.Billing) && t.Name == "LastName").Id;
            var shippingId =
                orderAddressMapper.OptionTypes
                    .First(t => (t.IdObjectType == null || t.IdObjectType == (int) AddressType.Shipping) && t.Name == "LastName").Id;
            var items = data.Select(p => new GCWithOrderListItemModel(p, billingId, shippingId));
            if (filter.ShippingAddress != null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
            {
                items =
                    items.Where(
                        p =>
                            !String.IsNullOrEmpty(p.ShippingLastName) &&
                            p.ShippingLastName.IndexOf(filter.ShippingAddress.LastName, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
            {
                items =
                    items.Where(
                        p =>
                            !String.IsNullOrEmpty(p.BillingLastName) &&
                            p.BillingLastName.IndexOf(filter.BillingAddress.LastName, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            GCStatisticModel toReturn = new GCStatisticModel();
            List<GCWithOrderListItemModel> resultList = items.ToList();
            toReturn.Count = resultList.Count;
            toReturn.Total = resultList.Sum(p => p.Balance);
            if (filter.Paging != null)
            {
                resultList =
                    resultList.Skip((filter.Paging.PageIndex - 1)*filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount).ToList();

                //var balanceConditions  =  new GcQuery().NotDeleted().WithFrom(filter.From).WithTo(filter.To).WithType(filter.Type).
                //    WidthStatus(RecordStatusCode.Active);
                //if (filter.ShippingAddress != null && !String.IsNullOrEmpty(filter.ShippingAddress.LastName))
                //{
                //    balanceConditions = conditions.WithShippingAddress(filter.ShippingAddress);
                //}
                //if (filter.BillingAddress != null && !String.IsNullOrEmpty(filter.BillingAddress.LastName))
                //{
                //    balanceConditions = conditions.WithBillingAddress(filter.BillingAddress);
                //}
                //q = (Expression<Func<GiftCertificate, bool>>)queryVisitor.Visit(balanceConditions.Query());
                //toReturn.Total = await giftCertificateRepository.Query(q).SelectSumAsync(p => p.Balance);
            }
            toReturn.Items = resultList;

            foreach (var item in toReturn.Items)
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
            var query = giftCertificateRepository.Query(new GcQuery().WithEqualCode(code.Trim()).NotDeleted());

            return query.SelectFirstOrDefaultAsync(false);
        }

        public Task<List<GiftCertificate>> TryGetGiftCertificatesAsync(ICollection<string> codes)
        {
            if (codes == null || codes.Count == 0)
            {
                return Task.FromResult(new List<GiftCertificate>());
            }

            var query = giftCertificateRepository.Query(new GcQuery().WithEqualCodes(codes).NotDeleted());
            return query.SelectAsync(false);
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
                dbItem.IdEditedBy = model.IdEditedBy;
                dbItem.Tag = model.Tag;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }

                await giftCertificateRepository.UpdateAsync(dbItem);

                await _objectLogItemExternalService.LogItem(dbItem);
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
                item.IdEditedBy = model.IdEditedBy;
                item.UserId = model.UserId;
                item.Tag = model.Tag;
                items.Add(item);
            }

            await giftCertificateRepository.InsertRangeAsync(items);
            
            await _objectLogItemExternalService.LogItems(items);

            return items;
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

        public async Task<IList<string>> GenerateGCCodes(int count)
        {
            var toReturn = new List<string>();
            bool generated = false;
            while (!generated)
            {
                var attempts=new List<string>();
                for (int j = 0; j < count; j++)
                {
                    var attempt = string.Empty;
                    Guid guid = Guid.NewGuid();
                    var bytes = guid.ToByteArray();
                    for (int i = bytes.Length - 1; i >= bytes.Length - GC_SYMBOLS_COUNT; i--)
                    {
                        attempt += symbols[bytes[i] % symbols.Count];
                    }
                    attempts.Add(attempt);
                }

                var dbItems = await giftCertificateRepository.Query(p => attempts.Contains(p.Code)).SelectAsync(false);
                var localItems = toReturn.Intersect(attempts).ToList();
                localItems.AddRange(dbItems.Select(p=>p.Code));

                var temp = attempts.Except(localItems).ToList();
                toReturn.AddRange(temp);
                count -= temp.Count();
                if (localItems.Count==0)
                {
                    generated = true;
                }
            }
            return toReturn;
        }

        public async Task<ICollection<GiftCertificate>> ImportGCsAsync(byte[] file, int idAddedBy, GCImportNotificationType? notificationType)
        {
            List<GCImportItem> records = new List<GCImportItem>();
            Dictionary<string, ImportItemValidationGenericProperty> validationSettings = null;
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var configuration = new CsvConfiguration();
                    configuration.ConfigureDefault<GCImportItemCsvMap>();
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        PropertyInfo[] modelProperties = typeof(GCImportItem).GetProperties();
                        validationSettings = BusinessHelper.GetAttrBaseImportValidationSettings(modelProperties);

                        int rowNumber = 1;
                        try
                        {
                            while (csv.Read())
                            {
                                if (rowNumber > FileConstants.MAX_IMPORT_ROWS_COUNT)
                                {
                                    throw new AppValidationException($"File for import cannot contain more than { FileConstants.MAX_IMPORT_ROWS_COUNT}");
                                }

                                var item = csv.GetRecord<GCImportItem>();
                                item.RowNumber = rowNumber;
                                var localMessages = new List<MessageInfo>();
                                rowNumber++;

                                var expirationDateProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(GCImportItem.ExpirationDate));
                                var expirationDateHeader = expirationDateProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;

                                var sExpirationDate = csv.GetField<string>(expirationDateHeader);
                                if (!String.IsNullOrEmpty(sExpirationDate))
                                {
                                    DateTime tempDate;
                                    if (DateTime.TryParse(sExpirationDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                                    {
                                        tempDate = TimeZoneInfo.ConvertTime(tempDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
                                        if (notificationType == GCImportNotificationType.StandartAdminEGiftEmail)
                                        {
                                            localMessages.Add(BusinessHelper.AddErrorMessage(expirationDateHeader,
                                                String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldShouldBeBlack], expirationDateHeader)));
                                        }
                                        else
                                        { 
                                            if (tempDate < DateTime.Now)
                                            {
                                                localMessages.Add(BusinessHelper.AddErrorMessage(expirationDateHeader, 
                                                    String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.MustBeFutureDateError], expirationDateHeader)));
                                            }
                                            else
                                            {
                                                item.ExpirationDate = tempDate;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        localMessages.Add(BusinessHelper.AddErrorMessage(expirationDateHeader, 
                                            String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseDateError], expirationDateHeader)));
                                    }
                                }
                                else
                                {
                                    if(notificationType==GCImportNotificationType.ExpirationDateAdminEGiftEmail)
                                    {
                                        localMessages.Add(BusinessHelper.AddErrorMessage(expirationDateHeader, 
                                            String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], expirationDateHeader)));
                                    }
                                }

                                var amountProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(GCImportItem.Balance));
                                var amountBaseHeader = amountProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;
                                var sAmount = csv.GetField<string>(amountBaseHeader);
                                if (!String.IsNullOrEmpty(sAmount))
                                {
                                    sAmount = sAmount.Replace("$", "");
                                    decimal tempAmount=0;
                                    if (Decimal.TryParse(sAmount, out tempAmount))
                                    {
                                        if (tempAmount <= 0 || tempAmount > 1000)
                                        {
                                            localMessages.Add(BusinessHelper.AddErrorMessage(amountBaseHeader, 
                                                String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.GCImportAmountRestrictions], 0, 1000)));
                                        }
                                        else
                                        {
                                            item.Balance = tempAmount;
                                        }
                                    }
                                    else
                                    {
                                        localMessages.Add(BusinessHelper.AddErrorMessage(amountBaseHeader, 
                                            String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseIntError], amountBaseHeader)));
                                    }
                                }
                                else
                                {
                                    localMessages.Add(BusinessHelper.AddErrorMessage(amountBaseHeader, 
                                        String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], amountBaseHeader)));
                                }

                                item.ErrorMessages = localMessages;
                                records.Add(item);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e.ToString());
                            throw new AppValidationException(e.Message);
                        }
                    }
                }
            }

            if (validationSettings != null)
            {
                BusinessHelper.ValidateAttrBaseImportItems(records, validationSettings);
            }

            //throw parsing and validation errors
            var messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var now=DateTime.Now;
            var toReturn = new List<GiftCertificate>();
            var codes = await GenerateGCCodes(records.Count);
            for (int i = 0; i < records.Count; i++)
            {
                var gcImportItem = records[i];

                var item = new GiftCertificate()
                {
                    FirstName = gcImportItem.FirstName,
                    LastName = gcImportItem.LastName,
                    Email = gcImportItem.Email,
                    Balance = gcImportItem.Balance,
                    Tag = gcImportItem.Tag,
                    ExpirationDate = gcImportItem.ExpirationDate,

                    GCType = GCType.EGC,
                    Created = now,
                    StatusCode = RecordStatusCode.Active,
                    IdEditedBy = idAddedBy,
                    UserId = idAddedBy,
                    Code = codes[i],
                };
                toReturn.Add(item);
            }

            await giftCertificateRepository.InsertRangeAsync(toReturn);

            await _objectLogItemExternalService.LogItems(toReturn);

            return toReturn;
        }
    }
}