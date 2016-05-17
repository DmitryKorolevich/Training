using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;

namespace VC.Admin.Models.Healthwise
{
    public class HealthwisePeriodListItemModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? PaidAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public int OrdersCount { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public decimal AverageDiscountedSubtotal { get; set; }

        public DateTime? LastOrderDate { get; set; }

        public bool AllowPayment { get; set; }

        public HealthwisePeriodListItemModel(VHealthwisePeriod item)
        {
            if(item!=null)
            {
                Id = item.Id;
                StartDate = item.StartDate;
                EndDate = item.EndDate;
                PaidAmount = item.PaidAmount;
                PaidDate = item.PaidDate;
                OrdersCount = item.OrdersCount;
                DiscountedSubtotal = item.DiscountedSubtotal;
                LastOrderDate = item.LastOrderDate;
                AverageDiscountedSubtotal = OrdersCount!= 0 ? Math.Round(DiscountedSubtotal/OrdersCount, 2) : 0;
            }
        }
    }
}
