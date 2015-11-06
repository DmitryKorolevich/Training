using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public class VAffiliate : Entity
	{
        public RecordStatusCode StatusCode { get; set; }

	    public string Name{ get; set; }

        public int CustomersCount { get; set; }

        public string Company { get; set; }

        public string WebSite { get; set; }

	    public decimal CommissionFirst { get; set; }

	    public decimal CommissionAll { get; set; }

        public string Tier { get; set; }

        public DateTime DateEdited { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedByAgentId { get; set; }
    }
}
