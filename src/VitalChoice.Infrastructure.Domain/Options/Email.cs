namespace VitalChoice.Infrastructure.Domain.Options
{
    public class Email
    {
        public string From { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
        public bool Secured { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}