using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class ShortOrderListItemModel : BaseModel
    {
        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Total { get; set; }

        public DateTime DateEdited { get; set; }

        public OrderType IdObjectType { get; set; }

        public ShortOrderListItemModel(Order item)
        {
            if(item!=null)
            {
                Id = item.Id;
                OrderStatus = item.OrderStatus;
                DateCreated = item.DateCreated;
                Total = item.Total;
                DateEdited = item.DateEdited;
                IdObjectType = (OrderType)item.IdObjectType;
            }
        }
    }
}