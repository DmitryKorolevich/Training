using System;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class WholesaleListitem
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string Company { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? IdTradeClass { get; set; }

        public string TradeClass { get; set; }

        public int? IdTier { get; set; }

        public string Tier { get; set; }

        public decimal SalesLastThreeMonths { get; set; }

        public decimal SalesLastYear { get; set; }

        public DateTime? LastOrderDate { get; set; }
    }
}