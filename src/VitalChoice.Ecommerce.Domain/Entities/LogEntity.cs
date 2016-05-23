namespace VitalChoice.Ecommerce.Domain.Entities
{
    public abstract class LogEntity : Entity
    {
        public RecordStatusCode StatusCode { get; set; }

        public int? IdEditedBy { get; set; }
    }
}