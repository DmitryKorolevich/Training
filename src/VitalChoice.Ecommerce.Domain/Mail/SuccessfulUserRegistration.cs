namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class SuccessfulUserRegistration : EmailTemplateDataModel
    {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileLink { get; set; }
	}
}
