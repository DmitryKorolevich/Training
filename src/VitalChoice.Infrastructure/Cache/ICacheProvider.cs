namespace VitalChoice.Infrastructure.Cache
{
    public interface ICacheProvider
    {
	    void SetItem(string key, object value, int minutes = 60);

	    object GetItem(string key);

	    T GetItem<T>(string key);

	    void Remove(string key);
    }
}