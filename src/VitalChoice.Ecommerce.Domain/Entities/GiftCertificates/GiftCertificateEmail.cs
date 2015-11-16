namespace VitalChoice.Ecommerce.Domain.Entities.GiftCertificates
{
    public class GiftCertificateEmail : Entity
    {
        public string ToName { get; set; }

        public string ToEmail { get; set; }

        public string FromName { get; set; }

        public string Message { get; set; }
    }
}