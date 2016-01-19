namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class PasswordReset : EmailTemplateDataModel
    {
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	    public string Link { get; set; }
    }
}