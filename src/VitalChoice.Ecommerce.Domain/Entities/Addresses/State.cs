namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
    public class State : Entity
    {
        public string StateCode { get; set; }

        public string CountryCode { get; set; }

        public string StateName { get; set; }

        public int Order { get; set; }

        public RecordStatusCode StatusCode { get; set; }
    }
}
