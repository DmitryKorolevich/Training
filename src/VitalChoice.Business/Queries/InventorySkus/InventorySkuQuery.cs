using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.InventorySkus
{
    public class InventorySkuQuery : QueryObject<InventorySku>
    {
        public InventorySkuQuery WithId(int id)
        {
            Add(x => x.Id == id);

            return this;
        }

        public InventorySkuQuery WithIds(ICollection<int> ids)
        {
            if (ids != null)
            {
                Add(x => ids.Contains(x.Id));
            }

            return this;
        }

        public InventorySkuQuery NotDeleted()
        {
            Add(x => x.StatusCode != (int)RecordStatusCode.Deleted);

            return this;
        }

        public InventorySkuQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode == (int)status);
            }

            return this;
        }

        public InventorySkuQuery WithCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code.Contains(code));
            }

            return this;
        }

        public InventorySkuQuery WithExactCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code==code);
            }

            return this;
        }

        public InventorySkuQuery WithDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                Add(x => x.Description.Contains(description));
            }

            return this;
        }
    }
}