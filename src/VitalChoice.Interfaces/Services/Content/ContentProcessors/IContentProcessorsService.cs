namespace VitalChoice.Interfaces.Services.Content.ContentProcessors
{
	public interface IContentProcessorsService
	{
        IContentProcessor GetContentProcessorByName(string name);
	}
}
