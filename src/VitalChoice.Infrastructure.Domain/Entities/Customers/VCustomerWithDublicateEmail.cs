using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class VCustomerWithDublicateEmail : Entity
    {
        public string Email { get; set; }

        public int Count { get; set; }
    }
}
