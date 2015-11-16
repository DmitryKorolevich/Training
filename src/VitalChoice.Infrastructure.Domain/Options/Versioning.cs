namespace VitalChoice.Infrastructure.Domain.Options
{
    public class Versioning
	{
        public bool EnableStaticContentVersioning { get; set; }

        public string BuildNumber { get; set; }
	}
}