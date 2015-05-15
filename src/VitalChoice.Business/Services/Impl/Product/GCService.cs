using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Business.Services.Impl.Content.ContentProcessors;
using Templates;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Business.Services.Contracts.Product;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Business.Mail;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Impl.Product
{
    public class GCService : IGCService
    {
        private const int GC_SYMBOLS_COUNT = 12;

        private static readonly List<char> symbols = new List<char>() { '0','1','2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'T', 'U', 'V', 'W', 'X','Y','Z' };

        private readonly IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;

        public GCService(IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository, IEmailSender emailSender)
        {
            this.giftCertificateRepository = giftCertificateRepository;
            this.emailSender = emailSender;
            logger = LoggerService.GetDefault();
        }

        public async Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter)
        {
            var query = new GCQuery().NotDeleted().WithType(filter.GCType).WithCode(filter.Code);
            PagedList<GiftCertificate> toReturn = await giftCertificateRepository.Query(query).OrderBy(x => x.OrderByDescending(pp => pp.Created)).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }

        public async Task<GiftCertificate> GetGiftCertificateAsync(int id)
        {
            GCQuery query = new GCQuery().WithId(id).NotDeleted();
            return (await giftCertificateRepository.Query(query).SelectAsync(false)).FirstOrDefault();
        }

        public async Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model)
        {
            GCQuery query = new GCQuery().WithId(model.Id).NotDeleted();
            GiftCertificate dbItem = (await giftCertificateRepository.Query(query).SelectAsync()).FirstOrDefault();

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

                dbItem = await giftCertificateRepository.UpdateAsync(dbItem);
            }

            return dbItem;
        }

        public async Task<ICollection<GiftCertificate>> AddGiftCertificatesAsync(int quantity, GiftCertificate model)
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
                item.Code = GenerateGCCode();
                items.Add(item);
            }

            await giftCertificateRepository.InsertRangeAsync(items);

            return items;
        }

        public async Task<bool> SendGiftCertificateEmailAsync(GiftCertificateEmail model)
        {
            await emailSender.SendEmailAsync(model.ToEmail, model.ToName, model.Message, model.FromName, model.ToName);
            return true;
        }

        public async Task<bool> DeleteGiftCertificateAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await giftCertificateRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await giftCertificateRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #region Private

        public string GenerateGCCode()
        {
            string toReturn = null;
            bool generated = false;
            while (!generated)
            {
                var attempt = String.Empty;
                Guid guid = Guid.NewGuid();
                var bytes = guid.ToByteArray();
                for(int i= bytes.Length-1; i >= bytes.Length- GC_SYMBOLS_COUNT; i--)
                {
                    attempt += symbols[bytes[i] % symbols.Count];
                }
                GiftCertificate dbItem = (giftCertificateRepository.Query(p=>p.Code== attempt).Select(false)).FirstOrDefault();
                if (dbItem == null)
                {
                    generated = true;
                    toReturn = attempt;
                }
            }
            return toReturn;
        }

        #endregion
    }
}