using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class ImportItemValidationGenericProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public Func<object, object> Get { get; set; }
        public Type PropertyType { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsEmail { get; set; }
        public int? MaxLength { get; set; }
    }
}
