using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Orders;

namespace VC.Admin.Models.Order
{
    public class ShortOrderListItemModel : BaseModel
    {
        public int Id { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Total { get; set; }

        public DateTime DateEdited { get; set; }

        public OrderType IdObjectType { get; set; }

        public ShortOrderListItemModel(VitalChoice.Domain.Entities.eCommerce.Orders.Order item)
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