using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.CustomerFiles.Entities
{
    public class VCustomerOldFile
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string OriginalFileName { get; set; }

        public int IdCustomer { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
