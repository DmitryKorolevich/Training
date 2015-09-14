using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.PaymentMethod;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Transfer.Help;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Business.Queries.Help;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Business.Mail;

namespace VitalChoice.Business.Services.HelpService
{
	public class HelpService : IHelpService
    {
		private readonly IEcommerceRepositoryAsync<HelpTicket> _helpTicketRepository;
        private readonly IEcommerceRepositoryAsync<HelpTicketComment> _helpTicketCommentRepository;
        private readonly IEcommerceRepositoryAsync<VHelpTicket> _vHelpTicketRepository;
        private readonly IHttpContextAccessor _contextAccessor;
		private readonly EcommerceContext _context;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

		public HelpService(IEcommerceRepositoryAsync<HelpTicket> helpTicketRepository, IEcommerceRepositoryAsync<HelpTicketComment> helpTicketCommentRepository,
            IEcommerceRepositoryAsync<VHelpTicket> vHelpTicketRepository,
            IHttpContextAccessor contextAccessor, EcommerceContext context, IRepositoryAsync<AdminProfile> adminProfileRepository,
             ICustomerService customerService, INotificationService notificationService, ILoggerProviderExtended loggerProvider)
        {
            _helpTicketRepository = helpTicketRepository;
            _helpTicketCommentRepository = helpTicketCommentRepository;
            _vHelpTicketRepository = vHelpTicketRepository;
            _contextAccessor = contextAccessor;
			_context = context;
			_adminProfileRepository = adminProfileRepository;
            _customerService = customerService;
            _notificationService = notificationService;
            _logger = loggerProvider.CreateLoggerDefault();
		}

        public async Task<PagedList<VHelpTicket>> GetHelpTicketsAsync(VHelpTicketFilter filter)
        {
            var conditions = new VHelpTicketQuery().WithDateCreatedFrom(filter.From).WithDateCreatedTo(filter.To)
                .WithStatus(filter.StatusCode).WithPriority(filter.Priority);

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

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }
        
        public async Task<HelpTicket> GetHelpTicketAsync(int id)
        {
            var condition = new HelpTicketQuery().NotDeleted().WithId(id);
            var item = (await _helpTicketRepository.Query(condition).Include(x => x.Order).Include(x=>x.Comments).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.Comments = item.Comments.Where(p => p.StatusCode != RecordStatusCode.Deleted).ToList();

                var vCondition = new VHelpTicketQuery().WithId(id);
                var vItem = (await _vHelpTicketRepository.Query(vCondition).SelectAsync(false)).FirstOrDefault();
                if(vItem!=null)
                {
                    item.Customer = vItem.Customer;
                    item.IdCustomer = vItem.IdCustomer;
                    item.CustomerEmail = vItem.CustomerEmail;
                }

                var adminProfileCondition = new AdminProfileQuery().IdInRange(item.Comments.Where(x=>x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());
                var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).Include(p=>p.User).SelectAsync(false);
                foreach(var comment in item.Comments)
                {
                    comment.HelpTicket = item;
                    foreach (var adminProfile in adminProfiles)
                    {
                        if(comment.IdEditedBy==adminProfile.Id)
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

            if(adminId.HasValue)
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
            var item = (await _helpTicketCommentRepository.Query(condition).Include(x => x.Order).SelectAsync(false)).FirstOrDefault();

            if (item != null)
            {
                item.HelpTicket= await GetHelpTicketAsync(item.IdHelpTicket);

                var adminProfile = (await _adminProfileRepository.Query(p=>p.Id==item.IdEditedBy).SelectAsync(false)).FirstOrDefault();
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
                    DateTime now= DateTime.Now;
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
                    var helpTicket= (await _helpTicketRepository.Query(condition).SelectAsync()).FirstOrDefault();
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

                if(adminId.HasValue)
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
            if(helpTicket!=null)
            {
                await _notificationService.SendHelpTicketUpdatingEmailForCustomerAsync(helpTicket.CustomerEmail, helpTicket);

                return true;
            }
            return false;
        }
    }
}