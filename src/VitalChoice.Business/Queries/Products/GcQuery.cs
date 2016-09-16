using System;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.Queries.Product
{
    public class GcQuery : QueryObject<GiftCertificate>
    {
        public GcQuery WithId(int id)
        {
            Add(x => x.Id == id);
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

        public GcQuery WithEqualCode(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                Add(x => x.Code == code);
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
                    var fPart = items[0];
                    var lPart = items[1];
                    Add(x => x.FirstName.Contains(fPart) && x.LastName.Contains(lPart));
                }
                else if (items.Length > 0)
                {
                    var part = items[0];
                    Add(x => x.FirstName.Contains(part) || x.LastName.Contains(part));
                }
            }

            return this;
        }

        public GcQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public GcQuery WidthStatus(RecordStatusCode? status)
        {
            if (status.HasValue)
            {
                Add(x => x.StatusCode == status.Value);
            }

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

        public GcQuery WithFrom(DateTime? from)
        {
            if (from.HasValue)
            {
                Add(x => x.Created>=from.Value);
            }
            return this;
        }

        public GcQuery WithTo(DateTime? to)
        {
            if (to.HasValue)
            {
                Add(x => x.Created<to.Value);
            }
            return this;
        }

        public GcQuery WithShippingAddress(CustomerAddressFilter filter)
        {
            if (filter!=null)
            {
                Add(c => c.Order.ShippingAddress.WhenValues(filter, (int)AddressType.Shipping, ValuesFilterType.And,
                            CompareBehaviour.StartsWith));
            }
            return this;
        }

        public GcQuery WithBillingAddress(CustomerAddressFilter filter)
        {
            if (filter != null)
            {
                Add(c => c.Order.PaymentMethod.BillingAddress.WhenValues(filter, (int)AddressType.Billing, ValuesFilterType.And,
                            CompareBehaviour.StartsWith));
            }
            return this;
        }
    }
}