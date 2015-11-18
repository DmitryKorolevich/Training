﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Business.Queries.Help;
using VitalChoice.Business.Mail;
using VitalChoice.Infrastructure.Azure;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;
using VitalChoice.Infrastructure.Domain.Transfer.Help;

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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly EcommerceContext _context;
        private readonly VitalChoiceContext _infrastructureContext;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly IBlobStorageClient _storageClient;
        private static string _bugTicketFilesContainerName;
        private static string _bugTicketCommentFilesContainerName;
        private readonly ILogger _logger;

        public HelpService(IEcommerceRepositoryAsync<HelpTicket> helpTicketRepository,
            IEcommerceRepositoryAsync<HelpTicketComment> helpTicketCommentRepository,
            IEcommerceRepositoryAsync<VHelpTicket> vHelpTicketRepository,
            IRepositoryAsync<BugTicket> bugTicketRepository,
            IRepositoryAsync<BugTicketComment> bugTicketCommentRepository,
            IRepositoryAsync<BugFile> bugFileRepository,
            IHttpContextAccessor contextAccessor,
            EcommerceContext context,
            VitalChoiceContext infrastructureContext,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ICustomerService customerService,
            INotificationService notificationService,
            IBlobStorageClient storageClient,
            IOptions<AppOptions> appOptions,
            ILoggerProviderExtended loggerProvider)
        {
            _helpTicketRepository = helpTicketRepository;
            _helpTicketCommentRepository = helpTicketCommentRepository;
            _vHelpTicketRepository = vHelpTicketRepository;
            _bugTicketRepository = bugTicketRepository;
            _bugTicketCommentRepository = bugTicketCommentRepository;
            _bugFileRepository = bugFileRepository;
            _contextAccessor = contextAccessor;
            _context = context;
            _infrastructureContext = infrastructureContext;
            _adminProfileRepository = adminProfileRepository;
            _customerService = customerService;
            _notificationService = notificationService;
            _storageClient = storageClient;
            _bugTicketFilesContainerName = appOptions.Value.AzureStorage.BugTicketFilesContainerName;
            _bugTicketCommentFilesContainerName = appOptions.Value.AzureStorage.BugTicketCommentFilesContainerName;
            _logger = loggerProvider.CreateLoggerDefault();
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
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VHelpTicketSortPath.IdOrder:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdOrder)
                                : x.OrderByDescending(y => y.IdOrder);
                    break;
                case VHelpTicketSortPath.Priority:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Priority)
                                : x.OrderByDescending(y => y.Priority);
                    break;
                case VHelpTicketSortPath.Summary:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Summary)
                                : x.OrderByDescending(y => y.Summary);
                    break;
                case VHelpTicketSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case VHelpTicketSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VHelpTicketSortPath.Customer:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Customer)
                                : x.OrderByDescending(y => y.Customer);
                    break;
                case VHelpTicketSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
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
            using (var transaction = new TransactionAccessor(_context).BeginTransaction())
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
                        item.IdOrder = dbItem.IdOrder;
                        item.DateCreated = dbItem.DateCreated;
                        item.DateEdited = DateTime.Now;
                        await _helpTicketRepository.UpdateAsync(item);
                    }

                    transaction.Commit();
                }
                catch (Exception)
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
                item.EditedBy = adminProfile.AgentId;
            }

            return item;
        }

        public async Task<HelpTicketComment> UpdateHelpTicketCommentAsync(HelpTicketComment item)
        {
            item.StatusCode = RecordStatusCode.Active;

            using (var transaction = new TransactionAccessor(_context).BeginTransaction())
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
                        item.Order = dbItem.Order;
                        item.IdHelpTicket = dbItem.IdHelpTicket;
                        item.DateCreated = dbItem.DateCreated;
                        item.DateEdited = now;
                        await _helpTicketCommentRepository.UpdateAsync(item);
                    }

                    var condition = new HelpTicketQuery().NotDeleted().WithId(item.IdHelpTicket);
                    var helpTicket = (await _helpTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                    helpTicket.DateEdited = now;
                    await _helpTicketRepository.UpdateAsync(helpTicket);

                    transaction.Commit();
                }
                catch (Exception)
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
                item.StatusCode = RecordStatusCode.Deleted;
                item.IdEditedBy = adminId;
                item.DateEdited = DateTime.Now;

                await _helpTicketCommentRepository.UpdateAsync(item);

                var condition = new HelpTicketQuery().NotDeleted().WithId(item.IdHelpTicket);
                var helpTicket = (await _helpTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                helpTicket.DateEdited = DateTime.Now;
                await _helpTicketRepository.UpdateAsync(helpTicket);

                if (adminId.HasValue)
                {
                    await NotifyCustomer(item.IdHelpTicket);
                }

                return true;
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

        public async Task<PagedList<BugTicket>> GetBugTicketsAsync(VHelpTicketFilter filter)
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
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case BugTicketSortPath.Priority:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Priority)
                                : x.OrderByDescending(y => y.Priority);
                    break;
                case BugTicketSortPath.Summary:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Summary)
                                : x.OrderByDescending(y => y.Summary);
                    break;
                case BugTicketSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case BugTicketSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case BugTicketSortPath.IdAddedBy:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdAddedBy)
                                : x.OrderByDescending(y => y.IdAddedBy);
                    break;
                case BugTicketSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var adminProfileCondition = new AdminProfileQuery().IdInRange(toReturn.Items.Select(x => x.IdAddedBy).ToList());
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

                var adminProfilesForTicket = await _adminProfileRepository.Query(p => p.Id == item.IdAddedBy).Include(p => p.User).SelectAsync(false);
                if (adminProfilesForTicket != null && adminProfilesForTicket.Count > 0)
                {
                    item.AddedBy = adminProfilesForTicket[0].User.FirstName + " " + adminProfilesForTicket[0].User.LastName;
                    item.AddedByAgent = adminProfilesForTicket[0].AgentId;
                    item.AddedByEmail = adminProfilesForTicket[0].User.Email;
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
            using (var transaction = new TransactionAccessor(_infrastructureContext).BeginTransaction())
            {
                try
                {
                    if (item.Id == 0)
                    {
                        item.StatusCode = RecordStatusCode.Active;
                        item.DateCreated = item.DateEdited = DateTime.Now;
                        item.IdAddedBy = item.IdEditedBy = adminId;
                        await _bugTicketRepository.InsertGraphAsync(item);
                        await _notificationService.SendNewBugTicketAddingForSuperAdminAsync(new BugTicketEmail { Id = item.Id });

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

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            if (item.IdAddedBy != adminId)
            {
                await NotifyAuthor(item.Id);
            }

            return item;
        }

        public async Task<bool> DeleteBugTicketAsync(int id, int? userId = null)
        {
            var item = (await _bugTicketRepository.Query(new BugTicketQuery().NotDeleted().WithId(id)).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                if(userId.HasValue && userId.Value!=item.IdAddedBy)
                {
                    return false;
                }

                item.StatusCode = RecordStatusCode.Deleted;
                item.DateEdited = DateTime.Now;

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

            using (var transaction = new TransactionAccessor(_infrastructureContext).BeginTransaction())
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
                        item.Order = dbItem.Order;
                        item.IdBugTicket = dbItem.IdBugTicket;
                        item.DateCreated = dbItem.DateCreated;
                        item.PublicId = dbItem.PublicId;
                        item.DateEdited = now;
                        await _bugTicketCommentRepository.UpdateAsync(item);
                    }

                    var condition = new BugTicketQuery().NotDeleted().WithId(item.IdBugTicket);
                    bugTicket = (await _bugTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
                    bugTicket.DateEdited = now;
                    await _bugTicketRepository.UpdateAsync(bugTicket);

                    transaction.Commit();
                }
                catch (Exception)
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

            if (bugTicket.IdAddedBy != item.IdEditedBy)
            {
                await NotifyAuthor(item.IdBugTicket);
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
                bugTicket.DateEdited = DateTime.Now;
                await _bugTicketRepository.UpdateAsync(bugTicket);

                if (bugTicket.IdAddedBy != adminId)
                {
                    await NotifyAuthor(item.IdBugTicket);
                }

                return true;
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
            } while (await _storageClient.BlobExistsAsync(containerName, blobname));

            await _storageClient.UploadBlobAsync(containerName, blobname, file, contentType);

            return generatedFileName;
        }

        public async Task<Blob> DownloadBugFileAsync(BugFileType type, string fileName, string publicId)
        {
            return await _storageClient.DownloadBlobAsync(GetFilesContainerName(type), $"{publicId}/{fileName}");
        }

        public async Task<bool> DeleteBugFileFromStoreAsync(BugFileType type, string fileName, string publicId)
        {
            return await _storageClient.DeleteBlobAsync(GetFilesContainerName(type), $"{publicId}/{fileName}");
        }

        #endregion
    }
}