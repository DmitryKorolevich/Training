using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Business.Queries.Helps;
using VitalChoice.Business.Mail;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;
using VitalChoice.Infrastructure.Domain.Transfer.Help;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Business.Services.HelpService
{
    public class HelpService : IHelpService
    {
        private readonly IEcommerceRepositoryAsync<HelpTicket> _helpTicketRepository;
        private readonly IEcommerceRepositoryAsync<HelpTicketComment> _helpTicketCommentRepository;
        private readonly IEcommerceRepositoryAsync<VHelpTicket> _vHelpTicketRepository;
        private readonly IRepositoryAsync<BugTicket> _bugTicketRepository;
        private readonly IRepositoryAsync<BugTicketComment> _bugTicketCommentRepository;
        private readonly IRepositoryAsync<BugFile> _bugFileRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly INotificationService _notificationService;
        private readonly IBlobStorageClient _storageClient;
        private static string _bugTicketFilesContainerName;
        private static string _bugTicketCommentFilesContainerName;
        private readonly ILogger _logger;
        private readonly ITransactionAccessor<EcommerceContext> _ecommerceTransactionAccessor;
        private readonly ITransactionAccessor<VitalChoiceContext> _infrastructureTransactionAccessor;

        public HelpService(IEcommerceRepositoryAsync<HelpTicket> helpTicketRepository,
            IEcommerceRepositoryAsync<HelpTicketComment> helpTicketCommentRepository,
            IEcommerceRepositoryAsync<VHelpTicket> vHelpTicketRepository,
            IRepositoryAsync<BugTicket> bugTicketRepository,
            IRepositoryAsync<BugTicketComment> bugTicketCommentRepository,
            IRepositoryAsync<BugFile> bugFileRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            INotificationService notificationService,
            IBlobStorageClient storageClient,
            IOptions<AppOptions> appOptions,
            ILoggerProviderExtended loggerProvider, ITransactionAccessor<EcommerceContext> ecommerceTransactionAccessor,
            ITransactionAccessor<VitalChoiceContext> infrastructureTransactionAccessor)
        {
            _helpTicketRepository = helpTicketRepository;
            _helpTicketCommentRepository = helpTicketCommentRepository;
            _vHelpTicketRepository = vHelpTicketRepository;
            _bugTicketRepository = bugTicketRepository;
            _bugTicketCommentRepository = bugTicketCommentRepository;
            _bugFileRepository = bugFileRepository;
            _adminProfileRepository = adminProfileRepository;
            _notificationService = notificationService;
            _storageClient = storageClient;
            _ecommerceTransactionAccessor = ecommerceTransactionAccessor;
            _infrastructureTransactionAccessor = infrastructureTransactionAccessor;
            _bugTicketFilesContainerName = appOptions.Value.AzureStorage.BugTicketFilesContainerName;
            _bugTicketCommentFilesContainerName = appOptions.Value.AzureStorage.BugTicketCommentFilesContainerName;
            _logger = loggerProvider.CreateLogger<HelpService>();
        }

        #region HelpTickets

        public async Task<PagedList<VHelpTicket>> GetHelpTicketsAsync(VHelpTicketFilter filter)
        {
            var conditions = new VHelpTicketQuery().WithDateCreatedFrom(filter.From).WithDateCreatedTo(filter.To)
                .WithStatus(filter.StatusCode).WithPriority(filter.Priority).WithIdOrder(filter.IdOrder).WithIdCustomer(filter.IdCustomer);

            var query = _vHelpTicketRepository.Query(conditions);

            Func<IQueryable<VHelpTicket>, IOrderedQueryable<VHelpTicket>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VHelpTicketSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VHelpTicketSortPath.IdOrder:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.IdOrder)
                                : x.OrderByDescending(y => y.IdOrder);
                    break;
                case VHelpTicketSortPath.Priority:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Priority)
                                : x.OrderByDescending(y => y.Priority);
                    break;
                case VHelpTicketSortPath.Summary:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Summary)
                                : x.OrderByDescending(y => y.Summary);
                    break;
                case VHelpTicketSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case VHelpTicketSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VHelpTicketSortPath.Customer:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Customer)
                                : x.OrderByDescending(y => y.Customer);
                    break;
                case VHelpTicketSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
            }
            PagedList<VHelpTicket> toReturn;
            if (filter.Paging != null)
            {
                toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await query.OrderBy(sortable).SelectAsync(false);
                toReturn = new PagedList<VHelpTicket>()
                {
                    Count = items.Count,
                    Items = items,
                };
            }
            return toReturn;
        }

        public async Task<HelpTicket> GetHelpTicketAsync(int id)
        {
            var condition = new HelpTicketQuery().NotDeleted().WithId(id);
            var item = (await _helpTicketRepository.Query(condition).Include(x => x.Order).Include(x => x.Comments).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.Comments = item.Comments.Where(p => p.StatusCode != RecordStatusCode.Deleted).ToList();

                var vCondition = new VHelpTicketQuery().WithId(id);
                var vItem = (await _vHelpTicketRepository.Query(vCondition).SelectAsync(false)).FirstOrDefault();
                if (vItem != null)
                {
                    item.Customer = vItem.Customer;
                    item.IdCustomer = vItem.IdCustomer;
                    item.CustomerEmail = vItem.CustomerEmail;
                }

                var adminProfileCondition = new AdminProfileQuery().IdInRange(item.Comments.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());
                var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).Include(p => p.User).SelectAsync(false);
                foreach (var comment in item.Comments)
                {
                    comment.HelpTicket = item;
                    foreach (var adminProfile in adminProfiles)
                    {
                        if (comment.IdEditedBy == adminProfile.Id)
                        {
                            comment.EditedBy = adminProfile.AgentId;
                        }
                    }
                }
                item.Comments = item.Comments.OrderBy(p => p.Order).ToList();
            }

            return item;
        }

        public async Task<HelpTicket> UpdateHelpTicketAsync(HelpTicket item, int? adminId)
        {
            using (var transaction = _ecommerceTransactionAccessor.BeginTransaction())
            {
                try
                {
                    if (item.Id == 0)
                    {
                        item.StatusCode = RecordStatusCode.Active;
                        item.DateCreated = item.DateEdited = DateTime.Now;
                        await _helpTicketRepository.InsertAsync(item);
                    }
                    else
                    {
                        var dbItem = (await _helpTicketRepository.Query(p => p.Id == item.Id).SelectAsync(false)).FirstOrDefault();
                        if (dbItem != null)
                        {
                            item.IdOrder = dbItem.IdOrder;
                            item.DateCreated = dbItem.DateCreated;
                            item.DateEdited = DateTime.Now;
                            await _helpTicketRepository.UpdateAsync(item);
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            if (adminId.HasValue)
            {
                await NotifyCustomer(item.Id);
            }

            return item;
        }

        public async Task<bool> DeleteHelpTicketAsync(int id)
        {
            var item = (await _helpTicketRepository.Query(new HelpTicketQuery().NotDeleted().WithId(id)).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.StatusCode = RecordStatusCode.Deleted;
                item.DateEdited = DateTime.Now;

                await _helpTicketRepository.UpdateAsync(item);

                return true;
            }

            return false;
        }

        public async Task<HelpTicketComment> GetHelpTicketCommentAsync(int id)
        {
            var condition = new HelpTicketCommentQuery().NotDeleted().WithId(id);
            var item = (await _helpTicketCommentRepository.Query(condition).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.HelpTicket = await GetHelpTicketAsync(item.IdHelpTicket);

                var adminProfile = (await _adminProfileRepository.Query(p => p.Id == item.IdEditedBy).SelectAsync(false)).FirstOrDefault();
                item.EditedBy = adminProfile?.AgentId;
            }

            return item;
        }

        public async Task<HelpTicketComment> UpdateHelpTicketCommentAsync(HelpTicketComment item)
        {
            item.StatusCode = RecordStatusCode.Active;

            using (var transaction = _ecommerceTransactionAccessor.BeginTransaction())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    if (item.Id == 0)
                    {
                        var comments = (await _helpTicketCommentRepository.Query(p => p.IdHelpTicket == item.IdHelpTicket).SelectAsync(false)).ToList();
                        if (comments.Count == 0)
                        {
                            item.Order = 0;
                        }
                        else
                        {
                            item.Order = comments.Max(p => p.Order) + 1;
                        }

                        item.DateCreated = item.DateEdited = now;
                        await _helpTicketCommentRepository.InsertAsync(item);
                    }
                    else
                    {
                        var dbItem = (await _helpTicketCommentRepository.Query(p => p.Id == item.Id).SelectAsync(false)).FirstOrDefault();

                        if (dbItem != null)
                        {
                            if ((dbItem.IdEditedBy.HasValue && !item.IdEditedBy.HasValue) ||
                                (!dbItem.IdEditedBy.HasValue && item.IdEditedBy.HasValue))
                            {
                                throw new Exception("The help ticket can't be updated by the given user.");
                            }

                            item.Order = dbItem.Order;
                            item.IdHelpTicket = dbItem.IdHelpTicket;
                            item.DateCreated = dbItem.DateCreated;
                            item.DateEdited = now;
                            await _helpTicketCommentRepository.UpdateAsync(item);
                        }
                    }

                    var condition = new HelpTicketQuery().NotDeleted().WithId(item.IdHelpTicket);
                    var helpTicket = (await _helpTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                    if (helpTicket != null)
                    {
                        helpTicket.DateEdited = now;
                        await _helpTicketRepository.UpdateAsync(helpTicket);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            if (item.IdEditedBy.HasValue)
            {
                var adminProfileCondition = new AdminProfileQuery().WithId(item.IdEditedBy.Value);
                var adminProfile = (await _adminProfileRepository.Query(adminProfileCondition).Include(p => p.User).SelectAsync(false)).FirstOrDefault();
                if (adminProfile != null)
                {
                    item.EditedBy = adminProfile.AgentId;
                }

                await NotifyCustomer(item.IdHelpTicket);
            }

            return item;
        }

        public async Task<bool> DeleteHelpTicketCommentAsync(int id, int? adminId)
        {
            var item = (await _helpTicketCommentRepository.Query(new HelpTicketCommentQuery().NotDeleted().WithId(id)).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                if((adminId.HasValue && !item.IdEditedBy.HasValue) || (!adminId.HasValue && item.IdEditedBy.HasValue))
                {
                    throw new Exception("The help ticket can't be updated by the given user.");
                }
                item.StatusCode = RecordStatusCode.Deleted;
                item.IdEditedBy = adminId;
                item.DateEdited = DateTime.Now;

                await _helpTicketCommentRepository.UpdateAsync(item);

                var condition = new HelpTicketQuery().NotDeleted().WithId(item.IdHelpTicket);
                var helpTicket = (await _helpTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                if (helpTicket != null)
                {
                    helpTicket.DateEdited = DateTime.Now;
                    await _helpTicketRepository.UpdateAsync(helpTicket);

                    if (adminId.HasValue)
                    {
                        await NotifyCustomer(item.IdHelpTicket);
                    }
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> NotifyCustomer(int idHelpTicket)
        {
            var helpTicket = await GetHelpTicketAsync(idHelpTicket);
            if (helpTicket != null)
            {
                await _notificationService.SendHelpTicketUpdatingEmailForCustomerAsync(helpTicket.CustomerEmail, new HelpTicketEmail()
                {
                    Id=helpTicket.Id,
                    IdOrder=helpTicket.IdOrder,
                    Customer=helpTicket.Customer,
                });

                return true;
            }
            return false;
        }

        #endregion

        #region BugTickets

        public async Task<PagedList<BugTicket>> GetBugTicketsAsync(BugTicketFilter filter)
        {
            var conditions = new BugTicketQuery().NotDeleted().WithDateCreatedFrom(filter.From).WithDateCreatedTo(filter.To)
                .WithStatus(filter.StatusCode).WithPriority(filter.Priority);

            var query = _bugTicketRepository.Query(conditions);

            Func<IQueryable<BugTicket>, IOrderedQueryable<BugTicket>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case BugTicketSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case BugTicketSortPath.Priority:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Priority)
                                : x.OrderByDescending(y => y.Priority);
                    break;
                case BugTicketSortPath.Summary:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Summary)
                                : x.OrderByDescending(y => y.Summary);
                    break;
                case BugTicketSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case BugTicketSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case BugTicketSortPath.IdAddedBy:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.AddedByUser.FirstName).ThenBy(y => y.AddedByUser.LastName)
                                : x.OrderByDescending(y => y.AddedByUser.FirstName).ThenByDescending(y => y.AddedByUser.LastName);
                    break;
                case BugTicketSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var ids = toReturn.Items.Select(x => x.IdAddedBy).ToList();
            ids.AddRange(toReturn.Items.Select(x => x.IdEditedBy).ToList());
            var adminProfileCondition = new AdminProfileQuery().IdInRange(ids);
            var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).Include(p => p.User).SelectAsync(false);
            foreach (var item in toReturn.Items)
            {
                foreach (var adminProfile in adminProfiles)
                {
                    if (item.IdAddedBy == adminProfile.Id)
                    {
                        item.AddedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                        item.AddedByAgent = adminProfile.AgentId;
                        item.AddedByEmail = adminProfile.User.Email;
                    }
                    if (item.IdEditedBy == adminProfile.Id)
                    {
                        item.EditedByAgent = adminProfile.AgentId;
                    }
                }
            }
            return toReturn;
        }

        public async Task<BugTicket> GetBugTicketAsync(int id)
        {
            var condition = new BugTicketQuery().NotDeleted().WithId(id);
            var item = (await _bugTicketRepository.Query(condition).Include(x => x.Comments).ThenInclude(x => x.Files).Include(x => x.Files).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.Comments = item.Comments.Where(p => p.StatusCode != RecordStatusCode.Deleted).ToList();

                var adminProfilesForTicket = await _adminProfileRepository.Query(p => p.Id == item.IdAddedBy || p.Id==item.IdEditedBy).Include(p => p.User).SelectAsync(false);
                if (adminProfilesForTicket.Count > 0)
                {
                    foreach (var adminProfile in adminProfilesForTicket)
                    {
                        if (item.IdAddedBy == adminProfile.Id)
                        {
                            item.AddedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                            item.AddedByAgent = adminProfile.AgentId;
                            item.AddedByEmail = adminProfile.User.Email;
                        }
                        if (item.IdEditedBy == adminProfile.Id)
                        {
                            item.EditedByAgent = adminProfile.AgentId;
                        }
                    }
                }

                var adminProfileCondition = new AdminProfileQuery().IdInRange(item.Comments.Select(x => x.IdEditedBy).ToList());
                var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).Include(p => p.User).SelectAsync(false);
                foreach (var comment in item.Comments)
                {
                    comment.BugTicket = item;
                    foreach (var adminProfile in adminProfiles)
                    {
                        if (comment.IdEditedBy == adminProfile.Id)
                        {
                            comment.EditedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                            comment.EditedByAgent = adminProfile.AgentId;
                        }
                    }
                }
                item.Comments = item.Comments.OrderBy(p => p.Order).ToList();
            }

            return item;
        }

        public async Task<BugTicket> UpdateBugTicketAsync(BugTicket item, int adminId, bool? isSuperAdmin = null)
        {
            using (var transaction = _infrastructureTransactionAccessor.BeginTransaction())
            {
                try
                {
                    if (item.Id == 0)
                    {
                        item.StatusCode = BugTicketStatus.Active;
                        item.DateCreated = item.DateEdited = DateTime.Now;
                        item.IdAddedBy = item.IdEditedBy = adminId;
                        await _bugTicketRepository.InsertGraphAsync(item);
                        await _notificationService.SendBugTicketUpdaingForSuperAdminAsync(new BugTicketEmail { Id = item.Id });

                        var adminProfile = (await _adminProfileRepository.Query(p => p.Id == item.IdAddedBy).Include(p => p.User).SelectAsync(false)).FirstOrDefault();
                        if (adminProfile != null)
                        {
                            item.AddedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                            item.AddedByAgent = adminProfile.AgentId;
                            item.AddedByEmail = adminProfile.User.Email;
                        }
                    }
                    else
                    {
                        var dbItem = (await _bugTicketRepository.Query(p => p.Id == item.Id).SelectAsync(false)).FirstOrDefault();
                        if (dbItem != null)
                        {
                            if (isSuperAdmin.HasValue && !isSuperAdmin.Value && adminId != dbItem.IdAddedBy)
                            {
                                return item;
                            }
                            item.DateCreated = dbItem.DateCreated;
                            item.PublicId = dbItem.PublicId;
                            item.IdAddedBy = dbItem.IdAddedBy;
                            item.DateEdited = DateTime.Now;
                            item.IdEditedBy = adminId;
                            await _bugTicketRepository.UpdateAsync(item);
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            if (item.IdAddedBy != adminId)
            {
                await NotifyAuthor(item.Id);
            }
            else
            {
                await NotifyMainSuperAdmin(item.Id);
            }

            return item;
        }

        public async Task<bool> DeleteBugTicketAsync(int id,int adminId , int? userId = null)
        {
            var item = (await _bugTicketRepository.Query(new BugTicketQuery().NotDeleted().WithId(id)).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                if(userId.HasValue && userId.Value!=item.IdAddedBy)
                {
                    return false;
                }

                item.StatusCode = BugTicketStatus.Deleted;
                item.DateEdited = DateTime.Now;
                item.IdEditedBy = adminId;

                await _bugTicketRepository.UpdateAsync(item);

                return true;
            }

            return false;
        }

        public async Task<BugTicketComment> GetBugTicketCommentAsync(int id)
        {
            var condition = new BugTicketCommentQuery().NotDeleted().WithId(id);
            var item = (await _bugTicketCommentRepository.Query(condition).Include(x => x.Files).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.BugTicket = await GetBugTicketAsync(item.IdBugTicket);

                var adminProfile = (await _adminProfileRepository.Query(p => p.Id == item.IdEditedBy).SelectAsync(false)).FirstOrDefault();

                if (adminProfile != null)
                {
                    item.EditedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                    item.EditedByAgent = adminProfile.AgentId;
                }
            }

            return item;
        }

        public async Task<BugTicketComment> UpdateBugTicketCommentAsync(BugTicketComment item)
        {
            item.StatusCode = RecordStatusCode.Active;
            BugTicket bugTicket = null;

            using (var transaction = _infrastructureTransactionAccessor.BeginTransaction())
            {
                try
                {
                    DateTime now = DateTime.Now;
                    if (item.Id == 0)
                    {
                        var comments = (await _bugTicketCommentRepository.Query(p => p.IdBugTicket == item.IdBugTicket).SelectAsync(false)).ToList();
                        if (comments.Count == 0)
                        {
                            item.Order = 0;
                        }
                        else
                        {
                            item.Order = comments.Max(p => p.Order) + 1;
                        }

                        item.DateCreated = item.DateEdited = now;
                        await _bugTicketCommentRepository.InsertGraphAsync(item);
                    }
                    else
                    {
                        var dbItem = (await _bugTicketCommentRepository.Query(p => p.Id == item.Id).SelectAsync(false)).FirstOrDefault();
                        if (dbItem != null)
                        {
                            item.Order = dbItem.Order;
                            item.IdBugTicket = dbItem.IdBugTicket;
                            item.DateCreated = dbItem.DateCreated;
                            item.PublicId = dbItem.PublicId;
                            item.DateEdited = now;
                            await _bugTicketCommentRepository.UpdateAsync(item);
                        }
                    }

                    var condition = new BugTicketQuery().NotDeleted().WithId(item.IdBugTicket);
                    bugTicket = (await _bugTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                    if (bugTicket != null)
                    {
                        bugTicket.DateEdited = now;
                        bugTicket.IdEditedBy = item.IdEditedBy;
                        await _bugTicketRepository.UpdateAsync(bugTicket);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            var adminProfileCondition = new AdminProfileQuery().WithId(item.IdEditedBy);
            var adminProfile = (await _adminProfileRepository.Query(adminProfileCondition).Include(p => p.User).SelectAsync(false)).FirstOrDefault();
            if (adminProfile != null)
            {
                item.EditedBy = adminProfile.User.FirstName + " " + adminProfile.User.LastName;
                item.EditedByAgent = adminProfile.AgentId;
            }

            if (bugTicket != null)
            {
                if (bugTicket.IdAddedBy != item.IdEditedBy)
                {
                    await NotifyAuthor(item.IdBugTicket);
                }
                else
                {
                    await NotifyMainSuperAdmin(item.IdBugTicket);
                }
            }

            return item;
        }

        public async Task<bool> DeleteBugTicketCommentAsync(int id, int adminId)
        {
            var item = (await _bugTicketCommentRepository.Query(new BugTicketCommentQuery().NotDeleted().WithId(id)).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.StatusCode = RecordStatusCode.Deleted;
                item.IdEditedBy = adminId;
                item.DateEdited = DateTime.Now;

                await _bugTicketCommentRepository.UpdateAsync(item);

                var condition = new BugTicketQuery().NotDeleted().WithId(item.IdBugTicket);
                var bugTicket = (await _bugTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                if (bugTicket != null)
                {
                    bugTicket.DateEdited = DateTime.Now;
                    bugTicket.IdEditedBy = adminId;
                    await _bugTicketRepository.UpdateAsync(bugTicket);

                    if (bugTicket.IdAddedBy != adminId)
                    {
                        await NotifyAuthor(item.IdBugTicket);
                    }
                    else
                    {
                        await NotifyMainSuperAdmin(item.IdBugTicket);
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<BugFile> AddBugFileAsync(BugFile file)
        {
            file.UploadDate = DateTime.Now;
            await _bugFileRepository.InsertAsync(file);
            return file;
        }

        public async Task<bool> DeleteBugFileAsync(int id)
        {
            await _bugFileRepository.DeleteAsync(id);
            return true;
        }

        private async Task<bool> NotifyAuthor(int idBugTicket)
        {
            var bugTicket = await GetBugTicketAsync(idBugTicket);
            if (bugTicket != null)
            {
                await _notificationService.SendBugTicketUpdatingEmailForAuthorAsync(bugTicket.AddedByEmail, new BugTicketEmail()
                {
                    Id=bugTicket.Id,
                    Customer=bugTicket.AddedBy,
                });

                return true;
            }
            return false;
        }

        private async Task<bool> NotifyMainSuperAdmin(int idBugTicket)
        {
            var bugTicket = await GetBugTicketAsync(idBugTicket);
            if (bugTicket != null)
            {
                await _notificationService.SendBugTicketUpdaingForSuperAdminAsync(new BugTicketEmail { Id = bugTicket.Id });

                return true;
            }
            return false;
        }


        #endregion

        #region BugFiles

        private string GetFilesContainerName(BugFileType type)
        {
            string toReturn = null;
            switch (type)
            {
                case BugFileType.Ticket:
                    toReturn = _bugTicketFilesContainerName;
                    break;
                case BugFileType.Comment:
                    toReturn = _bugTicketCommentFilesContainerName;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return toReturn;
        }

        public async Task<string> UploadBugFileToStoreAsync(BugFileType type, byte[] file, string fileName, string publicId, string contentType = null)
        {
            var i = 0;
            string blobname;
            string generatedFileName;
            string containerName = GetFilesContainerName(type);
            do
            {
                generatedFileName = (i != 0 ? (i + "_") : string.Empty) + fileName;

                blobname = $"{publicId}/{generatedFileName}";
                i++;
            } while (await _storageClient.BlobBlockExistsAsync(containerName, blobname));

            await _storageClient.UploadBlobAsync(containerName, blobname, file, contentType);

            return generatedFileName;
        }

        public async Task<Blob> DownloadBugFileAsync(BugFileType type, string fileName, string publicId)
        {
            return await _storageClient.DownloadBlobBlockAsync(GetFilesContainerName(type), $"{publicId}/{fileName}");
        }

        public async Task<bool> DeleteBugFileFromStoreAsync(BugFileType type, string fileName, string publicId)
        {
            return await _storageClient.DeleteBlobAsync(GetFilesContainerName(type), $"{publicId}/{fileName}");
        }

        #endregion
    }
}