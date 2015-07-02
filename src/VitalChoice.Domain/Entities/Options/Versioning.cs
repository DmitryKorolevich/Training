namespace VitalChoice.Domain.Entities.Options
{
    public struct Versioning
	{
        public bool EnableStaticContentVersioning { get; set; }

        public string BuildNumber { get; set; }
    }
}