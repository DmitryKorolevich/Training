using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class InventoryCategoryQuery : QueryObject<InventoryCategory>
    {
        public InventoryCategoryQuery WithId(int id)
        {
            Add(x => x.Id == id);

            return this;
        }

        public InventoryCategoryQuery WithParentId(int id)
        {
            Add(x => x.ParentId == id);

            return this;
        }

        public InventoryCategoryQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public InventoryCategoryQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode == status);
            }

            return this;
        }
    }
}