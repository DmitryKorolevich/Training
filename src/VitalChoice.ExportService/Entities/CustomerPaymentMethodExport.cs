namespace VitalChoice.ExportService.Entities
{
    public class CustomerPaymentMethodExport : PaymentMethodExport
    {
        public int IdCustomer { get; set; }
        public int IdPaymentMethod { get; set; }
    }
}