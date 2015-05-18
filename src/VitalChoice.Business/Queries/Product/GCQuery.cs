using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Entities.Product;

namespace VitalChoice.Business.Queries.Product
{
    public class GCQuery : QueryObject<GiftCertificate>
    {
        public GCQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));

            return this;
        }

        public GCQuery WithCode(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                Add(x => x.Code.Contains(code));
            }

            return this;
        }

        public GCQuery WithEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                Add(x => x.Email.Contains(email));
            }

            return this;
        }

        public GCQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            { 
                var items = name.Split(' ');
                if(items.Length==1)
                {
                    Add(x => x.FirstName.Contains(items[0]) || x.LastName.Contains(items[0]));
                }
                else
                {
                    Add(x => x.FirstName.Contains(items[0]) && x.LastName.Contains(items[1]));
                }
            }

            return this;
        }

        public GCQuery NotDeleted()
        {
            Add(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));

            return this;
        }

        public GCQuery WithType(GCType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.GCType.Equals(type));
            }

            return this;
        }
    }
}