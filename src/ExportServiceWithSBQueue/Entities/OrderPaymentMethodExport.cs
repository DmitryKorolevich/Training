namespace ExportServiceWithSBQueue.Entities
{
    public class OrderPaymentMethodExport : PaymentMethodExport
    {
        public int IdOrder { get; set; }
    }
}