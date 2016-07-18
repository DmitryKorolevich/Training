namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryParserFactory
    {
        IQueryParser<T> GetQueryParser<T>();
    }
}
