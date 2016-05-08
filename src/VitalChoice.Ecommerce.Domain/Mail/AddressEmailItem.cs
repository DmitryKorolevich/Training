using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class AddressEmailItem
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public string Company { get; set; }

        [Map]
        public string FirstName { get; set; }

        [Map]
        public string LastName { get; set; }

        [Map]
        public string Address1 { get; set; }

        [Map]
        public string Address2 { get; set; }

        [Map]
        public string City { get; set; }

        [Map]
        public int IdCountry { get; set; }

        public string Country { get; set; }

        [Map]
        public string County { get; set; }

        [Map]
        public int IdState { get; set; }

        public string StateCodeOrCounty { get; set; }

        [Map]
        public string Zip { get; set; }

        [Map]
        public string Phone { get; set; }

        [Map]
        public string Fax { get; set; }

        [Map]
        public string Email { get; set; }

        [Map]
        public string DeliveryInstructions { get; set; }

        [Map]
        public PreferredShipMethod? PreferredShipMethod { get; set; }
    }
}
