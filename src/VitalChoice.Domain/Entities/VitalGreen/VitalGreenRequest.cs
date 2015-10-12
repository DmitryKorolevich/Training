using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Helpers.Export;

namespace VitalChoice.Domain.Entities.VitalGreen
{
    public class VitalGreenRequest : Entity, IExportable
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int? ZoneId { get; set; }

        public FedExZone Zone { get; set; }

        public DateTime DateView { get; set; }

        public DateTime DateCompleted { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }
    }
}
