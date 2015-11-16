using cloudscribe.Web.Pagination;
using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;

namespace VC.Public.Models.Affiliate
{
    public class OrderPaymentsModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int Count { get; set; }

        public ICollection<OrderPaymentListItemModel> Items { get; set; }

        public PaginationSettings Paging { get; set; }
    }
}
