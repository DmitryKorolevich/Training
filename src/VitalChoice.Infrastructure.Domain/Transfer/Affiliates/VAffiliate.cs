using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class VAffiliate : Entity
	{
        public AffiliateStatus StatusCode { get; set; }

	    public string Name{ get; set; }

        public string Email { get; set; }

        public int CustomersCount { get; set; }

        public string Company { get; set; }

        public string WebSite { get; set; }

	    public decimal CommissionFirst { get; set; }

	    public decimal CommissionAll { get; set; }

        public string Tier { get; set; }

        public DateTime DateEdited { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }

        public VAffiliateNotPaidCommission NotPaidCommission { get; set; }

	    public int? PaymentType { get; set; }
    }
}
