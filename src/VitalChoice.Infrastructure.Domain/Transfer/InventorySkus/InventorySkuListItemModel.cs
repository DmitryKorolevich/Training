using System;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuListItemModel
    {
        public int Id { get; set; }

        public int StatusCode { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }

        public int? Quantity { get; set; }

        public InventorySkuListItemModel(InventorySku item)
        {
            if(item!=null)
            {
                Id = item.Id;
                StatusCode = item.StatusCode;
                Code = item.Code;
                Description = item.Description;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                IdEditedBy = item.IdEditedBy;
            }
        }
    }
}