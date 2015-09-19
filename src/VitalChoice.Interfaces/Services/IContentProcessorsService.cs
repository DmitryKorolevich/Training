namespace VitalChoice.Interfaces.Services
{
	public interface IContentProcessorsService
	{
        IContentProcessor GetContentProcessorByName(string name);
	}
}