using System;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Business.Queries.Product
{
    public class GCConditions : SimpleConditions<GiftCertificate>
    {
        public GCConditions WithId(int id)
        {
            _queryFluent.Where(x => x.Id.Equals(id));

            return this;
        }

        public GCConditions WithCode(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                _queryFluent.Where(x => x.Code.Contains(code));
            }

            return this;
        }

        public GCConditions WithEmail(string email)
        {
            if (!String.IsNullOrEmpty(email))
            {
                _queryFluent.Where(x => x.Email.Contains(email));
            }

            return this;
        }

        public GCConditions WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var items = name.Split(' ');
                if (items.Length == 1)
                {
                    _queryFluent.Where(x => x.FirstName.Contains(items[0]) || x.LastName.Contains(items[0]));
                }
                else
                {
                    _queryFluent.Where(x => x.FirstName.Contains(items[0]) && x.LastName.Contains(items[1]));
                }
            }

            return this;
        }

        public GCConditions NotDeleted()
        {
            _queryFluent.Where(x => x.StatusCode==RecordStatusCode.Active || x.StatusCode==RecordStatusCode.NotActive);

            return this;
        }

        public GCConditions WithType(GCType? type)
        {
            if (type.HasValue)
            {
                _queryFluent.Where(x => x.GCType==type);
            }

            return this;
        }
    }
}