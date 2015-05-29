using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.Product
{
    public class Product : Entity
    {
        public RecordStatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public ProductType? IdProductType { get; set; }

        public int? IdExternal { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool Hidden { get; set; }

        public ProductTypeEntity ProductTypeEntity { get; set; }
    }
}