using System;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class GcQuery : QueryObject<GiftCertificate>
    {
        public GcQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));
            return this;
        }

        public GcQuery WithCode(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                Add(x => x.Code.Contains(code));
            }

            return this;
        }

        public GcQuery WithEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                Add(x => x.Email.Contains(email));
            }

            return this;
        }

        public GcQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var items = name.Split(' ');
                if (items.Length > 1)
                {
                    Add(x => x.FirstName.Contains(items[0]) && x.LastName.Contains(items[1]));
                }
                else
                {
                    Add(x => x.FirstName.Contains(items[0]) || x.LastName.Contains(items[0]));
                }
            }

            return this;
        }

        public GcQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public GcQuery WithType(GCType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.GCType == type);
            }

            return this;
        }
    }
}