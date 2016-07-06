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
            return (await _redirectRepository.Query(p => p.Id == id && p.StatusCode == RecordStatusCode.Active).SelectAsync(false)).FirstOrDefault();
        }

        public async Task<Redirect> UpdateRedirectAsync(Redirect item)
        {
            var dublicatesExist = await _redirectRepository.Query(p => p.From == item.From && p.Id != item.Id
                && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
            if (dublicatesExist)
            {
                throw new AppValidationException("From", "Redirect with the same Source URL already exists, please use a unique URL.");
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
                var dbItem= (await _redirectRepository.Query(p =>p.Id == item.Id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
                if(dbItem!=null)
                {
                    dbItem.IdEditedBy = item.IdEditedBy;
                    dbItem.DateEdited = DateTime.Now;
                    dbItem.From = item.From;
                    dbItem.To = item.To;
                    await _redirectRepository.UpdateAsync(dbItem);
                }
            }
            return item;
        }

        public async Task<bool> DeleteRedirectAsync(int id, int idUser)
        {
            var item = (await _redirectRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();

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
    }
}
