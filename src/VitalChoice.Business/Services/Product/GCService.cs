﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Interfaces.Services.Product;

namespace VitalChoice.Business.Services.Product
{
    public class GCService : IGCService
    {
        private const int GC_SYMBOLS_COUNT = 12;

        private static readonly List<char> symbols = new List<char>() { '0','1','2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'T', 'U', 'V', 'W', 'X','Y','Z' };

        private readonly IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;

        public GCService(IEcommerceRepositoryAsync<GiftCertificate> giftCertificateRepository, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            this.giftCertificateRepository = giftCertificateRepository;
            this.userManager = userManager;
            this.emailSender = emailSender;
            logger = LoggerService.GetDefault();
        }

        public async Task<PagedList<GiftCertificate>> GetGiftCertificatesAsync(GCFilter filter)
        {
            var query = giftCertificateRepository.Query();

            var conditions = new GCConditions();
            conditions.Init(query);
            conditions = conditions.NotDeleted().WithType(filter.Type).WithCode(filter.Code).WithEmail(filter.Email).WithName(filter.Name);

            PagedList<GiftCertificate> toReturn = await query.OrderBy(x => x.OrderByDescending(pp => pp.Created)).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var users = await userManager.Users.Where(p=>toReturn.Items.Select(pp=>pp.UserId).Contains(p.Id)).Include(x => x.Profile).ToListAsync();
            foreach(var item in toReturn.Items)
            {
                foreach(var user in users)
                {
                    if(item.UserId==user.Id)
                    {
                        item.User = user;
                        break;
                    }
                }
            }

            return toReturn;
        }

        public async Task<GiftCertificate> GetGiftCertificateAsync(int id)
        {
            var query = giftCertificateRepository.Query();

            var conditions = new GCConditions();
            conditions.Init(query);
            conditions = conditions.WithId(id).NotDeleted();

            return (await query.SelectAsync(false)).FirstOrDefault();
        }

        public async Task<GiftCertificate> UpdateGiftCertificateAsync(GiftCertificate model)
        {
            var query = giftCertificateRepository.Query();

            var conditions = new GCConditions();
            conditions.Init(query);
            conditions = conditions.WithId(model.Id).NotDeleted();

            GiftCertificate dbItem = (await query.SelectAsync()).FirstOrDefault();

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
            await emailSender.SendEmailAsync(model.ToEmail, "Your Vital Choice Gift Certificate(s)", model.Message, model.FromName, model.ToName, false);
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