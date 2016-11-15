using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IRepositoryAsync<Redirect> _redirectRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        
        public RedirectService(IRepositoryAsync<Redirect> redirectRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
        {
            _redirectRepository = redirectRepository;
            _adminProfileRepository = adminProfileRepository;
        }

        public async Task<PagedList<Redirect>> GetRedirectsAsync(FilterBase filter)
        {
            var toReturn = await _redirectRepository.Query(p => p.StatusCode == RecordStatusCode.Active).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
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
                        item.AddedBy = adminProfile.AgentId;
                    }
                    if (item.IdEditedBy == adminProfile.Id)
                    {
                        item.EditedBy = adminProfile.AgentId;
                    }
                }
            }
            return toReturn;
        }

        public async Task<Redirect> GetRedirectAsync(int id)
        {
            return (await _redirectRepository.Query(p => p.Id == id && p.StatusCode == RecordStatusCode.Active).SelectFirstOrDefaultAsync(false));
        }

        public async Task<Redirect> UpdateRedirectAsync(Redirect item)
        {
            var dublicatesExist = await _redirectRepository.Query(p => p.From == item.From && p.Id != item.Id
                && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
            if (dublicatesExist)
            {
                throw new AppValidationException("From", "Redirect with the same Source URL already exists, please use a unique URL.");
            }

            if (item.IgnoreQuery && item.From.Contains("?"))
            {
                throw new AppValidationException("From", "From field can't contain query params if \"Ignore Query Params\" option is specified.");
            }

            if (item.Id==0)
            {
                item.IdAddedBy = item.IdEditedBy;
                item.DateCreated = item.DateEdited =DateTime.Now;
                item.StatusCode = RecordStatusCode.Active;
                await _redirectRepository.InsertAsync(item);
            }
            else
            {
                var dbItem= (await _redirectRepository.Query(p =>p.Id == item.Id && p.StatusCode != RecordStatusCode.Deleted).SelectFirstOrDefaultAsync(false));
                if(dbItem!=null)
                {
                    dbItem.IdEditedBy = item.IdEditedBy;
                    dbItem.DateEdited = DateTime.Now;
                    dbItem.From = item.From;
                    dbItem.To = item.To;
                    dbItem.IgnoreQuery = item.IgnoreQuery;
                    dbItem.FutureRedirectData = item.FutureRedirectData;
                    await _redirectRepository.UpdateAsync(dbItem);
                }
            }
            return item;
        }

        public async Task<bool> DeleteRedirectAsync(int id, int idUser)
        {
            var item = (await _redirectRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));

            if (item != null)
            {
                item.StatusCode = RecordStatusCode.Deleted;
                item.DateEdited = DateTime.Now;
                item.IdEditedBy = idUser;

                await _redirectRepository.UpdateAsync(item);

                return true;
            }

            return false;
        }

        public async Task ChangeRedirectsBasedOnFutureRedirectsAsync()
        {
            var redirects = await _redirectRepository.Query(p => p.StatusCode == RecordStatusCode.Active && p.FutureRedirectData!=null).SelectAsync(false);
            var now = DateTime.Now;
            ICollection<Redirect> forUpdate=new List<Redirect>();

            foreach (var redirect in redirects)
            {
                var futureRedirects = redirect.FutureRedirects;
                var newRoute = futureRedirects.Where(p =>!p.Disabled && p.StartDate < now).OrderBy(p => p.StartDate).FirstOrDefault();
                if (newRoute != null)
                {
                    redirect.To = newRoute.Url;
                    newRoute.Disabled = true;
                    redirect.FutureRedirects = futureRedirects;

                    forUpdate.Add(redirect);
                }
            }

            if (forUpdate.Count > 0)
            {
                await _redirectRepository.UpdateRangeAsync(forUpdate);
            }
        }
    }
}
